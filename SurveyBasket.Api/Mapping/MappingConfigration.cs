using SurveyBasket.Api.Contracts.Question;

namespace SurveyBasket.Api.Mapping;

public class MappingConfigration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // throw new NotImplementedException();
        config.NewConfig<QuestionRequest, Question>()
             .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));
            //.Ignore(nameof(Question.Answers));
    }
}
