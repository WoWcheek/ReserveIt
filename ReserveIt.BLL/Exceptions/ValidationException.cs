﻿namespace ReserveIt.BLL.Exceptions;

public class ValidationException : ApiException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message, 400)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred", 400)
    {
        Errors = errors;
    }
}
