using FluentValidation;
using System.Net;

namespace iPractice.Domain.Aspects.Validation;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithHttpStatusCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        HttpStatusCode code)
    {
        return rule.WithErrorCode(((int)code).ToString());
    }
}
