using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

namespace RestPanda.Requests;

/// <summary>
/// Request class
/// </summary>
public class PandaRequest
{
    private HttpListenerRequest _request;
    public Dictionary<string, string> Params { get; } = new ();

    /// <summary>
    /// Main request ctor
    /// </summary>
    /// <param name="request">Http request</param>
    internal PandaRequest(HttpListenerRequest request)
    {
        _request = request;
    }
    /// <summary>
    /// Request constructor with parameters
    /// </summary>
    /// <param name="request">Http request</param>
    /// <param name="param">Request parameters</param>
    internal PandaRequest(HttpListenerRequest request, string param) : this(request)
    {
        var s = param.Split('&');
        foreach (var keys in s)
        {
            var d = keys.Split('=');
            Params[d[0]] = d.Length == 2 ? d[1] : "";
        }
    }

    /// <summary>
    /// Getting the request body
    /// </summary>
    /// <param name="body">Request Body</param>
    /// <returns>Is there a request body</returns>
    public bool TryGetBody(out string body)
    {
        body = "";
        if (!_request.HasEntityBody) return false;
        var bodyStream = _request.InputStream;
        var encoding = _request.ContentEncoding;
        var reader = new StreamReader(bodyStream, encoding);

        body = reader.ReadToEnd();
        bodyStream.Close();
        reader.Close();
        return true;
    }
    
    /// <summary>
    /// Get object from request
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <returns></returns>
    public T? GetObject<T>()
    {
        return (T?) GetObjectFromBody<T>();
    }

    private object? GetObjectFromBody<T>()
    {
        if (TryGetBody(out var body))
        {
            return JsonSerializer.Deserialize<T>(body);
        }

        return null;
    }
}