using System.Collections.ObjectModel;
using System.Reflection;

namespace RestPanda.Core.Utils;

internal class MethodsSet
{
    private readonly List<Method> Methods;

    internal MethodsSet()
    {
        Methods = new List<Method>();
    }

    internal void Add(Method method)
    {
        Methods.Add(method);
    }

    internal bool IsEmpty => !Methods.Any();
    internal bool Contains(string path, string httpMethod)
    {
        return Methods.Any(m => m.Path == path && m.HttpMethod == httpMethod);
    }

    internal Method GetValue(string path, string httpMethod)
    {
        return Methods.Single(m => m.Path == path && m.HttpMethod == httpMethod);
    }

    internal bool TryGetValue(string path, string httpMethod, out Method? method)
    {
        var m = Methods.FirstOrDefault(m => m.Path == path && m.HttpMethod == httpMethod);
        if (m is null)
        {
            method = null;
            return false;
        }

        method = m;
        return true;
    }
}