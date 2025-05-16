

namespace SurveyBasket.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    
    
    [HttpPost(template: "")]
    public async Task<IActionResult> Login(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);
        return authResult is null ? BadRequest("Invalid Email or Password") : Ok(authResult);
    }

    [HttpPost(template: "refresh")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(tokenRequest.Token, tokenRequest.RefreshToken, cancellationToken);
        return authResult is null ? BadRequest("Invalid Token") : Ok(authResult);
    }

    [HttpPut(template: "revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync(RefreshTokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var isRevoked = await _authService.RevokeRefreshTokenAsync(tokenRequest.Token, tokenRequest.RefreshToken, cancellationToken);
        return isRevoked ? Ok(): BadRequest("Operation Failed");
    }
}
