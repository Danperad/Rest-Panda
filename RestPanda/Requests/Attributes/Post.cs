namespace RestPanda.Requests.Attributes;

/// <summary>
/// Request POST mark.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Post : Attribute, IRequest
{
    /// <summary>
    /// Request Endpoint.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initialize a new instance of the Post Attribute class with the specific path.
    /// </summary>
    /// <param name="path">The path of the request</param>
    public Post(string path)
    {
        Path = path;
        if (!Path.StartsWith("/") && Path.Length != 0) Path = "/" + Path;
        if (Path.Count(c => c == '/') > 1 || Path.Length == 1) throw new NotSupportedException("Path error");
    }

    /// <summary>
    /// Initialize a new instance of the Get Attribute class.
    /// </summary>
    private Post()
    {
        Path = "/";
    }
}