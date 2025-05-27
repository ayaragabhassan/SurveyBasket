using Microsoft.AspNetCore.Identity.Data;
using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost(template: "")]
    public async Task<IActionResult> Login(Contracts.Authorization.LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logging with email: {email} and password: {password}", loginRequest.Email, loginRequest.Password);

        var authResult = await _authService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);
        return authResult.Match(
            authResponse => Ok(authResponse),
            error => Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                extensions: new Dictionary<string,object?>
                {
                    { "errors",new object[] { error } }
                })
            );
    
    }

    [HttpPost(template: "refresh")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(tokenRequest.Token, tokenRequest.RefreshToken, cancellationToken);
        return authResult.IsFailure
            ? authResult.ToProblem() 
            : Ok(authResult.Value);
    }

    [HttpPut(template: "revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync(RefreshTokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(tokenRequest.Token, tokenRequest.RefreshToken, cancellationToken);
        
        return result.IsSuccess ? Ok()
            : result.ToProblem();
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Contracts.Authorization.RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ConfirmEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] Contracts.Authentication.ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResendConfirmationEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}
