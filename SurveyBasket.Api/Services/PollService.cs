using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Persistance;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDBContext context) : IPollService
{
    private readonly ApplicationDBContext _context = context;
   
    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var polls = await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        return polls.Adapt<IEnumerable<PollResponse>>();
    }
  
    public async Task<Result<PollResponse>> CreateAsync([FromBody]PollRequest request, CancellationToken cancellationToken)
    {
        var isExisted = await _context.Polls.AnyAsync(p=>p.Title == request.Title, cancellationToken);
        if (isExisted) {
           return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);
        }
        var poll = request.Adapt<Poll>();
        await _context.Polls.AddAsync(poll, cancellationToken);
    
        var created = await _context.SaveChangesAsync(cancellationToken);
        var pollResonse = poll.Adapt<PollResponse>();

        return created > 0 ?  Result.Sucess(pollResonse) : Result.Failure<PollResponse>(PollErrors.PollNotCreated);
    }

    public async Task<Result> UpdateAsync([FromRoute]int id,[FromBody] PollRequest request,CancellationToken cancellationToken)
    {
        var isExisted = await _context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != id, cancellationToken);
        if (isExisted)
        {
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);
        }
        var currentPoll = await _context.Polls.FindAsync(id, cancellationToken); ;
        if (currentPoll != null)
        {
            currentPoll.Title = request.Title;
            currentPoll.Summery = request.Summery;
            currentPoll.StartedAt = request.StartedAt;
            currentPoll.EndAt = request.EndAt;

            _context.Polls.Update(currentPoll);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Sucess();
        }
        return Result.Failure(PollErrors.PollNotFound);
    }

    public async Task<Result> DeleteAsync([FromRoute]int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken); ;
        if (poll != null)
        {
            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Sucess();
        }
        return Result.Failure(PollErrors.PollNotFound);
    }

    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
        if (poll != null)
        {
            poll.IsPublished = !poll.IsPublished;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Sucess();
        }
        return Result.Failure(PollErrors.PollNotFound);
    }

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
       
        return poll is not null ?  Result.Sucess(poll.Adapt<PollResponse>()) :
            Result.Failure<PollResponse>(PollErrors.PollNotFound); 
    }
}
