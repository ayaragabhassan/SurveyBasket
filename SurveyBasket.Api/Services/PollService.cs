
using Azure.Core;
using SurveyBasket.Api.Contracts.Polls.Request;
using SurveyBasket.Api.Persistance;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDBContext context) : IPollService
{
    private readonly ApplicationDBContext _context = context;
   
    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken)
        => await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
  
    public async Task<Poll?> GetAsync([FromRoute] int id,CancellationToken cancellationToken)
        => await _context.Polls.FindAsync(id, cancellationToken);

    public async Task<Poll> CreateAsync([FromBody]PollRequest request, CancellationToken cancellationToken)
    {
        var poll = request.Adapt<Poll>();
        await _context.Polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return poll;
    }

    public async Task<bool> UpdateAsync([FromRoute]int id,[FromBody] PollRequest request,CancellationToken cancellationToken)
    {
        var currentPoll = await GetAsync(id, cancellationToken);
        if (currentPoll != null)
        {
            currentPoll.Title = request.Title;
            currentPoll.Summery = request.Summery;
            currentPoll.IsPublished = request.IsPublished;
            currentPoll.StartedAt = request.StartedAt;
            currentPoll.EndAt = request.EndAt;

            _context.Polls.Update(currentPoll);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        return false;
    }

    public async Task<bool> DeleteAsync([FromRoute]int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetAsync(id,cancellationToken);
        if (poll != null)
        {
            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        return false;
    }

    public async Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetAsync(id, cancellationToken);
        if (poll != null)
        {
            poll.IsPublished = !poll.IsPublished;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        return false;
    }
}
