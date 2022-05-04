using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using RestPanda.Requests.Attributes;

namespace RestPanda.Requests;

public abstract class RequestHandler
{
    internal PandaRequest? Request { get; set; }
    internal PandaResponse? Response { get; set; }
    
    protected ReadOnlyDictionary<string, string> Params => Request is null
        ? new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())
        : Request.Params;

    protected ReadOnlyDictionary<string, string> Headers => Request is null
        ? new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())
        : Request.Headers;

    protected internal void FindHandler(string httpMethod, string path)
    {
        var type = GetType();
        var request = httpMethod switch
        {
            "GET" => typeof(Get),
            "POST" => typeof(Post),
            _ => null
        };
        if (request is null) return;
        foreach (var method in type.GetMethods())
        {
            var attribute = method.GetCustomAttribute(request);
            if (attribute == null) continue;
            if (((IRequest) attribute).Path != path) continue;
            method.Invoke(this, null);
        }
    }
    
    #region Response

    protected void AddHeader(HttpResponseHeader key, string value)
    {
        Response?.AddHeader(key, value);
    }

    protected void AddHeader(string key, string value)
    {
        Response?.AddHeader(key, value);
    }

    protected void Send(string answer)
    {
        if (Response is null) return;
        Response?.Send(answer);
        Request = null;
        Response = null;
    }

    protected void Send(object answer)
    {
        if (Response is null) return;
        Response?.Send(answer);
        Request = null;
        Response = null;
    }

    #endregion

    #region Request

    public bool TryGetBody(out string body)
    {
        body = "";
        return Request is not null && Request.TryGetBody(out body);
    }

    protected T? Bind<T>()
    {
        if (Request != null) return Request.GetObject<T>();
        throw new NullReferenceException();
    }

    #endregion
}