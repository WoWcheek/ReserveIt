﻿namespace ReserveIt.BLL.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message, 404)
    { }
}