using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using OneOf;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contracts.Authentication;
using SurveyBasket.Api.Helpers;
using System.Security.Cryptography;
using System.Text;


namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;
    private int refreshTokenExpiresIn = 14;

   
    public async Task<OneOf<AuthResponse,Error>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //Check email 
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return UserErrors.InvalidCredentials;
        }
        //Check Password
        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!isValidPassword)
        {
            return UserErrors.InvalidCredentials;
        }

        //Generate JWT Token 
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);


        //Genereate Refresh Token
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpires = DateTime.UtcNow.AddDays(refreshTokenExpiresIn);

        user.RefreshTokens.Add(new RefreshToken { Token = refreshToken, ExpiredOn = refreshTokenExpires });

        await _userManager.UpdateAsync(user);
        
        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpires);

        return response;

    }

   
    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId == null) { return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken); }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) { return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials); }

        var userRefreshToken =  user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken && r.IsActive);

        if (userRefreshToken == null) { return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken); }
        userRefreshToken.RevokedOn = DateTime.UtcNow;


        //Generate JWT Token 
        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);


        //Genereate Refresh Token
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpires = DateTime.UtcNow.AddDays(refreshTokenExpiresIn);

        user.RefreshTokens.Add(new RefreshToken { Token = newRefreshToken, ExpiredOn = refreshTokenExpires });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpires);
        return Result.Sucess(response);

    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId == null) 
            return Result.Failure(UserErrors.InvalidJwtToken); 

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Result.Failure(UserErrors.InvalidCredentials);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken && r.IsActive);

        if (userRefreshToken == null) 
             return Result.Failure(UserErrors.InvalidRefreshToken); 

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
       
        return Result.Sucess(user);
    }
    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public async Task<Result> RegisterAsync(Contracts.Authorization.RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailIsExists)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if(result.Succeeded)
        {
            //Generate Code Token 

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code: {code}", code);

            //Send Email with Confirmation 

            await SendConfirmationEmail(user, code);

            return Result.Sucess();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ResendConfirmationEmailAsync(Contracts.Authentication.ResendConfirmationEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Sucess();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation code: {code}", code);

        await SendConfirmationEmail(user, code);

        return Result.Sucess();
    }
    private async Task SendConfirmationEmail(ApplicationUser user, string code)
    {
        //origin link of app 
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" } //After Confirmation redirect to this link
            }
        );

        await _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Email Confirmation", emailBody);
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
            return Result.Sucess();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}
