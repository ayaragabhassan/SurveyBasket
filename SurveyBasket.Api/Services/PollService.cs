
using SurveyBasket.Api.Contracts.Request;
using SurveyBasket.Api.Persistance;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDBContext context) : IPollService
{
    private readonly ApplicationDBContext _context = context;
   
    public async Task<IEnumerable<Poll>> GetAllAsync()
        => await _context.Polls.AsNoTracking( ).ToListAsync();
  
    public async Task<Poll?> GetAsync(int id) => await _context.Polls.FindAsync(id);

    public async Task<Poll> CreateAsync(CreatePollRequest request)
    {
        var poll = request.Adapt<Poll>();
        await _context.Polls.AddAsync(poll);
        await _context.SaveChangesAsync();
        return poll;
    }

    //public bool Update(int id, Poll poll)
    //{
    //    var currentPoll = Get(id);
    //    if (currentPoll != null)
    //    {
    //        currentPoll.Title = poll.Title;
    //        currentPoll.Summery = poll.Summery;    
    //        return true;
    //    }
    //    return false;
    //}

    //public bool Delete(int id)
    //{
    //    var poll = Get(id);
    //    if (poll != null)
    //    {
    //        _polls.Remove(poll);
    //        return true;
    //    }
    //    return false;
    //}
}
