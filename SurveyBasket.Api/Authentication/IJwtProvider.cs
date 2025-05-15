namespace SurveyBasket.Api.Authentication;

public interface IJwtProvider
{
    (string token,int ExpiresIn) GenerateToken(ApplicationUser user);
}
