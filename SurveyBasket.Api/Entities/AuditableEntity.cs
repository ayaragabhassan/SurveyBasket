﻿namespace SurveyBasket.Api.Entities;

public class AuditableEntity
{
    public string CreatedById { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public string? UpdatedById { get; set; }

    public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

    public ApplicationUser CreatedBy { get; set; } = default!;

    public ApplicationUser? UpdatedBy { get; set; }
}
