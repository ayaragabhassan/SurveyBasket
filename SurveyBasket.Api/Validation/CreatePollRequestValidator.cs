
namespace SurveyBasket.Api.Validation;

public class CreatePollRequestValidator:AbstractValidator<Poll>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull()
            .GreaterThan(0);
    }
}
