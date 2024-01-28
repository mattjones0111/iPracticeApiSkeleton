using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using iPractice.Domain.Aspects.Validation;
using MediatR;

namespace iPractice.Domain.Pipeline;

public class DefaultPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : Response
    where TRequest : IRequest<TResponse>
{
    readonly IEnumerable<IValidator<TRequest>> validators;

    public DefaultPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Task<ValidationResult>[] validatorTasks = validators
            .Select(x => x.ValidateAsync(request, cancellationToken))
            .ToArray();

        ValidationResult[] results = await Task.WhenAll(validatorTasks);

        ValidationError[] failures = results
            .SelectMany(x => x.Errors)
            .Select(x => new ValidationError(x.PropertyName, x.ErrorMessage, ParseHttpError(x.ErrorCode)))
            .ToArray();

        if (failures.Any())
        {
            var x = Response.Invalid(failures);
            return (TResponse)x;
        }

        return await next();
    }

    static int ParseHttpError(string errorCode)
    {
        bool isHttpStatusCode = int.TryParse(errorCode, out int i) &&
                                i >= 100 &&
                                i <= 599;

        if (isHttpStatusCode)
        {
            return i;
        }

        return (int)HttpStatusCode.BadRequest;
    }
}
