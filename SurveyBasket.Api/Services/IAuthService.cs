﻿using OneOf;
using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Services;

public interface IAuthService
{
    //Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<OneOf<AuthResponse,Error>> GetTokenAsync(string email,string password,CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken);
    Task<Result> RegisterAsync(Contracts.Authorization.RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result> ResendConfirmationEmailAsync(Contracts.Authentication.ResendConfirmationEmailRequest request);
}
