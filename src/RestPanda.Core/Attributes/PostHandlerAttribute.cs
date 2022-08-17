namespace RestPanda.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PostHandlerAttribute : Attribute
{
    internal string Path { get; private set; }

    public PostHandlerAttribute() : this("/")
    {
    }

    public PostHandlerAttribute(string path)
    {
        Path = path;
    }
}