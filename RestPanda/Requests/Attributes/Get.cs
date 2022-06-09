namespace RestPanda.Requests.Attributes;

/// <summary>
/// Request GET mark.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Get : Attribute, IRequest
{
    /// <summary>
    /// Request Endpoint.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initialize a new instance of the Get Attribute class with the specific path.
    /// </summary>
    /// <param name="path">The path of the request</param>
    public Get(string path)
    {
        Path = path;
        if (!Path.StartsWith("/") && Path != "") Path = "/" + Path;
        if (Path.Count(c => c == '/') > 1) throw new NotSupportedException("Path error");
    }
    /// <summary>
    /// Initialize a new instance of the Get Attribute class.
    /// </summary>
    private Get()
    {
        Path = "/";
    }
}