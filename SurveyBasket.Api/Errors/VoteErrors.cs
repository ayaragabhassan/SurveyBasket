namespace SurveyBasket.Api.Errors;

public class VoteErrors
{
    public static readonly Error DuplicatedVote =
        new Error ("Vote.Duplicated", "You have already vote for this Poll",StatusCode:StatusCodes.Status409Conflict);
    public static readonly Error InvalidQuestions =
       new("Vote.InvalidQuestions", "Invalid questions", StatusCodes.Status400BadRequest);

}
