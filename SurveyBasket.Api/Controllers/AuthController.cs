

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
}
