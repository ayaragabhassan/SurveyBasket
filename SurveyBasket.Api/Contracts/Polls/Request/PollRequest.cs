namespace SurveyBasket.Api.Contracts.Polls.Request;

public record PollRequest
    (string Title,
     string Summery,
     DateOnly StartedAt,
     DateOnly EndAt);

