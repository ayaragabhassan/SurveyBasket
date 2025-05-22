using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Validations;

public class VoteRequestValidator :AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(v => v.Answers).NotEmpty();

        //to validate chaild elements
        RuleForEach(v => v.Answers).SetInheritanceValidator(v=>v.Add(new VoteAnswerRequestValidator()));
    }
}
