﻿namespace SurveyBasket.Api.Abstractions;

public record Error(string Code, string Name, int? StatusCode)
{
    public static readonly Error None = new Error(string.Empty, string.Empty,null);
}

