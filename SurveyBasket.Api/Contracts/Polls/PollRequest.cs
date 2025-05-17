namespace SurveyBasket.Api.Contracts.Polls;

public record PollRequest
    (string Title,
     string Summery,
     DateOnly StartedAt,
     DateOnly EndAt);

