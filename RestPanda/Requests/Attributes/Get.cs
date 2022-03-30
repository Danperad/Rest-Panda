namespace RestPanda.Requests.Attributes;

/// <summary>
/// Attribute for GET requests
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Get : Attribute, IRequest
{
    public string Path { get; }

    /// <summary>
    /// Attribute for GET requests
    /// </summary>
    /// <param name="path">The path of the request</param>
    public Get(string path)
    {
        Path = path;
        if (!Path.StartsWith("/") && Path != "") Path = "/" + Path;
    }
    public Get()
    {
        Path = "";
    }
}