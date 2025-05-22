using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Validations;

public class VoteAnswerRequestValidator :AbstractValidator<VoteAnswerRequest>
{
    public VoteAnswerRequestValidator()
    {
        RuleFor(v => v.AnswerId).NotEmpty().GreaterThan(0);
        RuleFor(v => v.QuestionId).NotEmpty().GreaterThan(0);
    }
}
