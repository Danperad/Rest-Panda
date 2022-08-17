using System.Collections.ObjectModel;
using System.Reflection;

namespace RestPanda.Core.Utils;

internal class Method
{
    internal ReadOnlyCollection<Parametr> Parametrs { get; }
    private readonly MethodInfo _methodInfo;
    private readonly object _instance;
    internal string Path { get; }
    internal string HttpMethod { get; }
    internal Type ReturnType { get; }

    public Method(string path, string httpMethod, MethodInfo method, object instance)
    {
        Path = path;
        HttpMethod = httpMethod;
        _methodInfo = method;
        ReturnType = _methodInfo.ReturnType;
        Parametrs = new ReadOnlyCollection<Parametr>(GetParams());
        _instance = instance;
    }

    private List<Parametr> GetParams()
    {
        var s = new List<Parametr>();
        var ss = _methodInfo.GetParameters();
        foreach (var parameterInfo in ss)
        {
            if (parameterInfo.IsOut) throw new NotSupportedException(); // TODO: Fix throwable 
            s.Add(new Parametr(parameterInfo));
        }

        return s;
    }

    private object? Invoke(params object[] p)
    {
        return _methodInfo.Invoke(_instance, p);
    }
}