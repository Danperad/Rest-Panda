namespace RestPanda.Exceptions;

/// <summary>
/// The exception thrown when creating a server without handlers.
/// </summary>
public class NoHandlersException : Exception
{
    /// <summary>
    /// Initialize a new instance of the NoHandlersException class.
    /// </summary>
    public NoHandlersException() : base("Handlers not found")
    {
    }

    /// <summary>
    /// Initialize a new instance of the EmptyUrlsException class with the specific message.
    /// </summary>
    /// <param name="msg">Message that describes the error.</param>
    public NoHandlersException(string msg) : base(msg)
    {
    }
}