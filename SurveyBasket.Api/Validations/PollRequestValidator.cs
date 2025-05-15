using SurveyBasket.Api.Contracts.Polls.Request;

namespace SurveyBasket.Contracts.Validations;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Summery)
            .NotEmpty()
            .Length(3, 1500);

        RuleFor(x => x.StartedAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(x => x.EndAt)
            .NotEmpty();

        RuleFor(x => x)
            .Must(HasValidDates)
            .WithName(nameof(PollRequest.EndAt))
            .WithMessage("{PropertyName} must be greater than or equals start date");
    }

    private bool HasValidDates(PollRequest pollRequest)
    {
        return pollRequest.EndAt >= pollRequest.StartedAt;
    }
}