namespace SurveyBasket.Api.Contracts.Request;

public record PollRequest
    (string Title,
     string Summery,
    bool IsPublished,
    DateOnly StartedAt,
    DateOnly EndAt)
{
}
