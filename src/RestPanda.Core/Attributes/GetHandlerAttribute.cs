namespace RestPanda.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class GetHandlerAttribute : Attribute
{
    internal string Path { get; private set; }

    public GetHandlerAttribute(): this("/")
    {
    }

    public GetHandlerAttribute(string path)
    {
        Path = path;
    }
}