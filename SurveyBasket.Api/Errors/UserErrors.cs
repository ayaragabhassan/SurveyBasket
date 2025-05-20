
namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredintials = new Error("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status400BadRequest);
}
