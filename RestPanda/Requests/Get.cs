namespace RestPanda.Requests;

[AttributeUsage(AttributeTargets.Method)]
public class Get : Attribute, IRequest
{
    public Get(string path)
    {
        Path = path;
    }

    public string Path { get; }
}