
using System.Xml;

namespace SurveyBasket.Api.ValidationAttributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MinAgeAttribute(int minAge) : ValidationAttribute
{
    private readonly int _minAge = minAge;
    //public override bool IsValid(object? value)
    //{
    //    if(value is not null)
    //    {
    //        var date = (DateTime)value;
    //        if(DateTime.Today < date.AddYears(_minAge))
    //            return false;
    //    }
    //    return true;
    //}

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not null)
        {
            var date = (DateTime)value;
            if (DateTime.Today < date.AddYears(_minAge))
                return new ValidationResult(errorMessage:$"Invalid {validationContext.DisplayName}, Age should be {_minAge}");
        }
        return ValidationResult.Success;
    }
}
