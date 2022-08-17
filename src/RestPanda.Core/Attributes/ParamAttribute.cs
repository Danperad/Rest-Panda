namespace RestPanda.Core.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class ParamAttribute : Attribute
{
    internal string Name { get; private set; }

    public ParamAttribute(string name)
    {
        Name = name;
    }
}