namespace RestPanda.Exceptions;

/// <summary>
/// The exception thrown when creating a server without addresses.
/// </summary>
public class EmptyUrlsException : Exception
{
    /// <summary>
    /// Initialize a new instance of the EmptyUrlsException class.
    /// </summary>
    public EmptyUrlsException() : base("Urls must be not empty"){}
    /// <summary>
    /// Initialize a new instance of the EmptyUrlsException class with the specific message.
    /// </summary>
    /// <param name="msg">Message that describes the error.</param>
    public EmptyUrlsException(string msg) : base(msg)
    {
    }
}