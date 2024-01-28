namespace iPractice.Domain.Aspects.Validation;

public record ValidationError(string Field, string Message, int Code = 400);
