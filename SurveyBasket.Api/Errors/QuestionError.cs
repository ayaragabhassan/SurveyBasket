namespace SurveyBasket.Api.Errors;

public class QuestionError
{
    public static readonly Error QuestionEmpty = new Error("Question.Empty", "The Question is empty", StatusCode: StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedQuestionConrent =
        new Error("Question.DuplicatedContent", "Another question with the same Content is already exists", StatusCode: StatusCodes.Status409Conflict);
    public static readonly Error QuestionNotFound = new Error("Question.NotFound", "No Question found with this given Id", StatusCode: StatusCodes.Status404NotFound);

}
