using SurveyBasket.Api.Contracts.Answer;
using SurveyBasket.Api.Contracts.Question;
using SurveyBasket.Api.Contracts.Votes;
using SurveyBasket.Api.Persistance;

namespace SurveyBasket.Api.Services;

public class VoteService(ApplicationDBContext context) : IVoteService
{
    public ApplicationDBContext _context { get; } = context;

    public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure(VoteErrors.DuplicatedVote);

        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartedAt 
        <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExists)
            return Result.Failure(PollErrors.PollNotFound);

        var availableQuestions = await _context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (!request.Answers.Select(x => x.QuestionId).SequenceEqual(availableQuestions))
            return Result.Failure(VoteErrors.InvalidQuestions);

        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await _context.AddAsync(vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Sucess();


    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvilableAsync(int pollId,string userId, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes.AnyAsync(v=>v.PollId == pollId &&  v.UserId == userId );

        if (hasVote)
            return Result.Failure <IEnumerable<QuestionResponse>> (VoteErrors.DuplicatedVote);

        var pollExisted = await _context.Polls.AnyAsync(p => p.Id == pollId);

        if (!pollExisted)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var avialbleQuestions = await _context.Questions.Where(q=>q.IsActive && q.PollId == pollId)
            .Include(q => q.Answers)
            .Select(question=> new QuestionResponse(

                question.Id,
                question.Content,
                question.Answers.Where(a=>a.IsActive).Select(
                a => new AnswerResponse (a.Id,  a.Content )
            ))).AsNoTracking().ToListAsync();

        return Result.Sucess<IEnumerable<QuestionResponse>>(avialbleQuestions);
    }
}
