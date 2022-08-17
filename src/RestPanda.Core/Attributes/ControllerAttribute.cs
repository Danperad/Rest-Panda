namespace RestPanda.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ControllerAttribute : Attribute
{
    internal string Path { get; private set; }

    public ControllerAttribute() : this("")
    {
    }

    public ControllerAttribute(string path)
    {
        Path = path;
    }
}