using SurveyBasket.Api.Contracts.Request;

namespace SurveyBasket.Api.Services;

public interface IPollService
{
    Task<IEnumerable<Poll>> GetAllAsync();
    Task<Poll?> GetAsync(int id);
    Task<Poll> CreateAsync(CreatePollRequest request);
    //bool Update(int id, Poll poll);
    //bool Delete(int id);
}
