using System.Collections.ObjectModel;
using System.Net;
using System.Text.Json;
using RestPanda.Exceptions;

namespace RestPanda.Requests;

/// <summary>
/// Request class
/// </summary>
internal class PandaRequest
{
    private readonly HttpListenerRequest _request;

    /// <summary>
    /// Request parameters
    /// </summary>
    internal ReadOnlyDictionary<string, string> Params { get; }

    /// <summary>
    /// Request headers
    /// </summary>
    internal ReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>
    /// Main request ctor
    /// </summary>
    /// <param name="request">Http request</param>
    internal PandaRequest(HttpListenerRequest request)
    {
        _request = request;
        Params = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        var temp = new Dictionary<string, string>();
        foreach (var key in _request.Headers.AllKeys)
        {
            temp[key!] = _request.Headers[key!]!;
        }

        Headers = new ReadOnlyDictionary<string, string>(temp);
    }

    private string? _body;
    /// <summary>
    /// Request constructor with parameters
    /// </summary>
    /// <param name="request">Http request</param>
    /// <param name="param">Request parameters</param>
    internal PandaRequest(HttpListenerRequest request, string param) : this(request)
    {
        var s = param.Split('&');
        var newParams = new Dictionary<string, string>();
        foreach (var keys in s)
        {
            var d = keys.Split('=');
            newParams[d[0]] = d.Length == 2 ? d[1] : "";
        }

        Params = new ReadOnlyDictionary<string, string>(newParams);
    }
    
    /// <summary>
    /// Getting the request body
    /// </summary>
    /// <param name="body">Request Body</param>
    /// <returns>Is there a request body</returns>
    internal bool TryGetBody(out string body)
    {
        body = "";
        if (_body != null)
        {
            body = _body;
            return true;
        }
        if (!_request.HasEntityBody) return false;
        try
        {
            using var bodyStream = _request.InputStream;
            var encoding = _request.ContentEncoding;
            using var reader = new StreamReader(bodyStream, encoding);
            _body = reader.ReadToEnd();
            body = _body;
            return body != "";
        }
        catch
        {
            _body = null;
            throw new EmptyBodyException();
        }
    }

    /// <summary>
    /// Get object from request
    /// </summary>
    /// <typeparam name="T">Type of object with empty constructor</typeparam>
    /// <returns></returns>
    internal T? GetObject<T>()
    {
        if (!TryGetBody(out var body)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(body);
        }
        catch (NotSupportedException e)
        {
            Console.WriteLine("Hint - T should contain an empty constructor");
            Console.WriteLine(e);
            throw;
        }
    }
}