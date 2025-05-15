namespace SurveyBasket.Api.Contracts.Responses;

public record PollResponse (int Id,
    string Title,
    string Summery,
    bool IsPublished,
    DateOnly StartedAt,
    DateOnly EndAt)
{
}
