using System;
using iPractice.Domain.Aspects.Validation;
using MediatR;

namespace iPractice.Domain.Pipeline;

public abstract class BaseRequest : IRequest<Response>
{
}

public class Response : IRequest
{
    public static Response Invalid(ValidationError[] errors) => new(errors);
    public static Response Created() => new();

    public ValidationError[] Errors { get; } = Array.Empty<ValidationError>();

    public bool IsCreated { get; }

    protected Response()
    {
        IsCreated = true;
    }

    Response(ValidationError[] errors)
    {
        Errors = errors;
    }
}

public class Response<TPayload> : Response
{
    public TPayload Payload { get; }

    public Response(TPayload payload)
    {
        Payload = payload;
    }
}
