namespace SurveyBasket.Api.Errors;

public class PollErrors
{
    public static readonly Error PollNotFound = new Error("Poll.NotFound", "No Poll found with this given Id",StatusCode:StatusCodes.Status404NotFound);
    public static readonly Error PollNotCreated = new Error("Poll.NotCreated", "No Poll Created", null);

}
