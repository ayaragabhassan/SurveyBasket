using SurveyBasket.Api.Contracts.Question;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Persistance;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDBContext context) : IQuestionService
{
    public ApplicationDBContext _context { get; } = context;

    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
    {
        var questionExists = await _context.Questions.AnyAsync(p => p.Id == questionId && p.PollId == pollId, cancellationToken);

        if (!questionExists)
            return Result.Failure<QuestionResponse>(QuestionError.QuestionNotFound);

        var question = await _context.Questions.Include(q => q.Answers).SingleOrDefaultAsync(q=>q.Id == questionId && q.PollId == pollId, cancellationToken);

        return Result.Sucess(question.Adapt<QuestionResponse>());

    }
    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!pollExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(q=>q.PollId == pollId)
            .Include(q=>q.Answers)
            //.Select(q=> new QuestionResponse(
            //    q.Id,
            //    q.Content,
            //    q.Answers.Select(a=>new Contracts.Answer.AnswerResponse(a.Id,a.Content))))
            .ProjectToType<QuestionResponse>()
            .AsNoTracking().ToListAsync(cancellationToken);


        return Result.Sucess(questions.AsEnumerable());

    }
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Result.Failure<QuestionResponse>(QuestionError.QuestionEmpty);
        var pollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);    

        if(!pollExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);
         
        var existedQuestionPoll = await _context.Questions.AnyAsync(q=>q.Content ==  request.Content&& q.PollId == pollId,cancellationToken);

        if (existedQuestionPoll)
            return Result.Failure<QuestionResponse>(QuestionError.DuplicatedQuestionConrent);

        var question = request.Adapt<Question>();

        question.PollId = pollId;

        //MapperConfig Do this 
        //foreach (var answer in request.Answers)
        //{
        //    question.Answers.Add(new Answer { Content = answer });
        //}

        await _context.AddAsync(question,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Sucess(question.Adapt<QuestionResponse>());

    
    }

    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Result.Failure(QuestionError.QuestionEmpty);
        var pollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!pollExists)
            return Result.Failure(PollErrors.PollNotFound);

        var duplicatedQuestionPoll = await _context.Questions.AnyAsync(q => q.Content == request.Content 
        && q.PollId == pollId
        && q.Id != id, cancellationToken);

        if (duplicatedQuestionPoll)
            return Result.Failure(QuestionError.DuplicatedQuestionConrent);

        var question = await _context.Questions.Include(q => q.Answers).SingleOrDefaultAsync(q => q.Id == id && q.PollId == pollId); 

        if(question is null)
            return Result.Failure(QuestionError.QuestionNotFound);

        
        question.Content = request.Content;
        foreach (var answer in request.Answers)
        {
            if (!question.Answers.Any(a => a.Content == answer))
            {
                question.Answers.Add(new Answer { Content = answer });
            }
        }
        question.Answers.ToList().ForEach(answer => {
            answer.IsActive = request.Answers.Contains(answer.Content);
        });

        _context.Questions.Update(question);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Sucess();
    }
    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var questionExists = await _context.Questions.SingleOrDefaultAsync(p => p.Id == id && p.PollId == pollId, cancellationToken);

        if (questionExists is null)
            return Result.Failure(QuestionError.QuestionNotFound);
        
        questionExists.IsActive = !questionExists.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Sucess();
    }
}
