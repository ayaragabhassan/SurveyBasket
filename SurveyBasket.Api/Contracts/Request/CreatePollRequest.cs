namespace SurveyBasket.Api.Contracts.Request;

public record CreatePollRequest(string Title,
     string Summery,
    bool IsPublished,
    DateOnly StartedAt,
    DateOnly EndAt)
{
}
