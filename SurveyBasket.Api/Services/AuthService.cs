using Microsoft.AspNetCore.Identity;
using OneOf;
using SurveyBasket.Api.Authentication;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
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

}
