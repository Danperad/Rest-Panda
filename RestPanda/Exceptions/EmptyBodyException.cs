namespace RestPanda.Exceptions;

/// <summary>
/// The exception thrown when trying to get a non-existent request body.
/// </summary>
public class EmptyBodyException : Exception
{
    /// <summary>
    /// Initialize a new instance of the EmptyBodyException class.
    /// </summary>
    public EmptyBodyException() : base("Body is empty or already received")
    {
    }

    /// <summary>
    /// Initialize a new instance of the EmptyBodyException class with the specific message.
    /// </summary>
    /// <param name="message">Message that describes the error.</param>
    public EmptyBodyException(string message) : base(message)
    {
    }
}