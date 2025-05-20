using SurveyBasket.Api.Contracts.Question;

namespace SurveyBasket.Api.Validations;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(q => q.Content)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(q => q.Answers)
            .NotNull();

        RuleFor(q => q.Answers)
            .Must(a => a.Count > 1)
            .WithMessage("Question should has at least two answers")
            .When(a=>a.Answers != null);

        RuleFor(q => q.Answers)
            .Must(a => a.Distinct().Count() == a.Count)
            .WithMessage("You can not added duplicated answer for the same question")
            .When(a => a.Answers != null);
    }
}
