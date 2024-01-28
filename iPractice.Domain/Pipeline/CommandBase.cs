using System;
using iPractice.Domain.Aspects.Validation;

namespace iPractice.Domain.Pipeline;

public class Response
{
    public static Response Invalid(ValidationError[] errors) => new(errors);
    public static Response Created() => new();
    public static Response WithPayload(object payload) => new(payload);

    public ValidationError[] Errors { get; } = Array.Empty<ValidationError>();

    public bool IsCreated { get; }

    public object Payload { get; }

    Response()
    {
        IsCreated = true;
    }

    Response(object payload)
    {
        Payload = payload;
    }

    Response(ValidationError[] errors)
    {
        Errors = errors;
    }
}
