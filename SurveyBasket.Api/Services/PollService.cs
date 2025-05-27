using Hangfire;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Persistance;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDBContext context,
    INotificationService notificationService) : IPollService
{
    private readonly ApplicationDBContext _context = context;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Polls.AsNoTracking().ProjectToType<PollResponse>().ToListAsync(cancellationToken); 
    }
    public async Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken)
    {
        var polls = await _context.Polls.AsNoTracking()
            .Where(p=>p.IsPublished && p.StartedAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);
        
        return polls;
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

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        poll.IsPublished = !poll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        if (poll.IsPublished && poll.StartedAt == DateOnly.FromDateTime(DateTime.UtcNow))
            BackgroundJob.Enqueue(() => _notificationService.SendNewPollsNotification(poll.Id));

        return Result.Sucess();
    }

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
       
        return poll is not null ?  Result.Sucess(poll.Adapt<PollResponse>()) :
            Result.Failure<PollResponse>(PollErrors.PollNotFound); 
    }
}
