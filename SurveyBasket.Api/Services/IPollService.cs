using SurveyBasket.Api.Contracts.Polls.Request;

namespace SurveyBasket.Api.Services;

public interface IPollService
{
    Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Poll> CreateAsync(PollRequest request,CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, PollRequest request,CancellationToken cancellationToken=default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default);

}
