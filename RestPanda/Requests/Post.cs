namespace RestPanda.Requests;

[AttributeUsage(AttributeTargets.Method)]
public class Post : Attribute, IRequest
{
    public string Path { get; }

    public Post(string path)
    {
        Path = path;
    }
}