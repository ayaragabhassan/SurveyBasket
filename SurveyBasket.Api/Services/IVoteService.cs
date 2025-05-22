using SurveyBasket.Api.Contracts.Question;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services;

public interface IVoteService
{
    Task<Result<IEnumerable<QuestionResponse>>> GetAvilableAsync(int pollId,string userId, CancellationToken cancellationToken = default);

    Task<Result> AddAsync(int pollId,string userId, VoteRequest Request, CancellationToken cancellationToken = default);
 

}
