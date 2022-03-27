namespace RestPanda.Exceptions;

public class EmptyUrlsException : Exception
{
    public EmptyUrlsException(string msg) : base(msg)
    {
    }
}