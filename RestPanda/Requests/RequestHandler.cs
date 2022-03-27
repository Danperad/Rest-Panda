namespace RestPanda.Requests;

[AttributeUsage(AttributeTargets.Class)]
public class RequestHandler : Attribute
{
    public string Path { get; }

    public RequestHandler(string path)
    {
        Path = path;
    }
}