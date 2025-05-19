

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
        return authResult.Match(
            authResponse => Ok(authResponse),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, title: error.Code, detail: error.Name)
            );
    
    }

    [HttpPost(template: "refresh")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(tokenRequest.Token, tokenRequest.RefreshToken, cancellationToken);
        return authResult.IsFailure
            ? Problem(statusCode: StatusCodes.Status400BadRequest, title: authResult.Error.Code, detail: authResult.Error.Name) 
            : Ok(authResult.Value);
    }

    [HttpPut(template: "revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync(RefreshTokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(tokenRequest.Token, tokenRequest.RefreshToken, cancellationToken);
        
        return result.IsSuccess ? Ok()
            : Problem(statusCode:StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Name);
    }
}
