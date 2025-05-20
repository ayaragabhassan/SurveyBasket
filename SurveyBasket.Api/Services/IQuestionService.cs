﻿using SurveyBasket.Api.Contracts.Question;

namespace SurveyBasket.Api.Services;

public interface IQuestionService
{
    Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> AddAsync(int pollId,QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int pollId,int id, CancellationToken cancellationToken = default);
}
