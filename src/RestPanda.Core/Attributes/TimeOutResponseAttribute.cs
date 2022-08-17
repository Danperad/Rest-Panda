namespace RestPanda.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TimeOutResponseAttribute : Attribute
{
    public double Time { get; private set; }

    public TimeOutResponseAttribute(double time)
    {
        if (time <= 0) throw new Exception();
        Time = time;
    }
}