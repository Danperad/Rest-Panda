namespace RestPanda.Exceptions;

public class NoHandlersException : Exception
{
    public NoHandlersException(string msg) : base(msg)
    {
    }
}