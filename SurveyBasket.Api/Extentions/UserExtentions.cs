﻿using System.Security.Claims;

namespace SurveyBasket.Api.Extentions;

public static class UserExtentions
{
    public static string? GetUserId(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier);
}
