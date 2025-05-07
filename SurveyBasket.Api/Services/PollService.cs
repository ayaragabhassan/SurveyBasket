
namespace SurveyBasket.Api.Services;

public class PollService : IPollService
{
    private static readonly List<Poll> _polls = [
        new Poll
        {
            Id = 1,
            Title = "Poll 1",
            Description = "My first poll"
        }
    ];

    public IEnumerable<Poll> GetAll() => _polls;
  
    public Poll? Get(int id) => _polls.SingleOrDefault(p => p.Id == id);

    public Poll Create(Poll poll)
    {
        poll.Id = _polls.Count+1;
        _polls.Add(poll);
        return poll;
    }

    public bool Update(int id, Poll poll)
    {
        var currentPoll = Get(id);
        if (currentPoll != null)
        {
            currentPoll.Title = poll.Title;
            currentPoll.Description = poll.Description;    
            return true;
        }
        return false;
    }

    public bool Delete(int id)
    {
        var poll = Get(id);
        if (poll != null)
        {
            _polls.Remove(poll);
            return true;
        }
        return false;
    }
}
