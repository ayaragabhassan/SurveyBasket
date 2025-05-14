namespace SurveyBasket.Api.Contracts.Responses;

public record PollResponse (int Id,
    string Tiltle,
    string Summery,
    bool IsPublished,
    DateOnly StartedAt,
    DateOnly EndAt)
{
}
