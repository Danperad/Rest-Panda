using System;

namespace RestPanda.Exceptions;

public class EmptyUrlsException : Exception
{
    public EmptyUrlsException(){}
    public EmptyUrlsException(string msg) : base(msg)
    {
    }
}