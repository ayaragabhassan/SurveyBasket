﻿namespace SurveyBasket.Api.Contracts.Authorization;

public record RegisterRequest(string Email,
    string Password,
    string FirstName,
    string LastName);

