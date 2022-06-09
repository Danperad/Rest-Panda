namespace RestPanda.Requests.Attributes;

/// <summary>
/// Request handler mark.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RequestHandlerPath : Attribute
{
    /// <summary>
    /// Path to request endpoint.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initialize a new instance of the Post Attribute class with the specific path.
    /// </summary>
    /// <param name="path">The path of the requests</param>
    public RequestHandlerPath(string path)
    {
        Path = path;
        if (Path.StartsWith("/")) Path = Path.Remove(0,1);
        if (Path.Contains('/') || string.IsNullOrWhiteSpace(Path)) throw new NotImplementedException("This path format is temporarily not supported");
    }
    /// <summary>
    /// Initialize a new instance of the RequestHandlerPath Attribute class.
    /// </summary>
    private RequestHandlerPath()
    {
        Path = "";
    }
}