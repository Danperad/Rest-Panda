using System.ComponentModel.DataAnnotations;
using System.Reflection;
using RestPanda.Core.Attributes;

namespace RestPanda.Core.Utils;

internal class Parametr
{
    internal Type ParamType { get; }
    internal string Name { get; }
    internal bool IsRequired { get; }
    internal bool IsBody { get; }
    internal bool IsHeader { get; }
    internal bool IsCookie { get; }
    internal bool IsResponse { get; }

    internal Parametr(ParameterInfo param)
    {
        Name = param.Name!;
        IsBody = param.GetCustomAttribute<BodyAttribute>() != null;
        IsRequired = param.GetCustomAttribute<RequiredAttribute>() != null;
        IsHeader = param.GetCustomAttribute<HeadersAttribute>() != null;
        IsCookie = param.GetCustomAttribute<CookiesAttribute>() != null;
        IsResponse = param.GetCustomAttribute<ResponseAttribute>() != null;
        if (IsBody || IsHeader || IsCookie || IsResponse)
        {
            ParamType = param.ParameterType;
            return;
        }
        
        if (Activator.CreateInstance(param.ParameterType) is not IConvertible)
            throw new NotSupportedException();
        ParamType = param.ParameterType;
    }
}