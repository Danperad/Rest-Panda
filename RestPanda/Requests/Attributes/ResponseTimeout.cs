namespace RestPanda.Requests.Attributes;

/// <summary>
/// Attribute for custom Time per method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ResponseTimeout : Attribute
{
    internal uint Time { get; }

    /// <summary>
    /// Initialize a new instance of the Timeout Attribute class with the specific time.
    /// </summary>
    /// <param name="time"></param>
    public ResponseTimeout(uint time)
    {
        Time = time;
    }
}