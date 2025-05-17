namespace SurveyBasket.Api.Contracts.Polls;

public record PollResponse (int Id,
    string Title,
    string Summery,
    bool IsPublished,
    DateOnly StartedAt,
    DateOnly EndAt)
{
}
