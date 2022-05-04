﻿using System.Collections.ObjectModel;
using System.Net;
using System.Text.Json;

namespace RestPanda.Requests;

/// <summary>
/// Request class
/// </summary>
public class PandaRequest
{
    private readonly HttpListenerRequest _request;
    public ReadOnlyDictionary<string, string> Params { get; private init; }
    public ReadOnlyDictionary<string, string> Headers { get; private init; }
    public Stream InputStream => _request.InputStream;

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

    /// <summary>
    /// Request constructor with parameters
    /// </summary>
    /// <param name="request">Http request</param>
    /// <param name="param">Request parameters</param>
    internal PandaRequest(HttpListenerRequest request, string param) : this(request)
    {
        var s = param.Split('&');
        var paramss = new Dictionary<string, string>();
        foreach (var keys in s)
        {
            var d = keys.Split('=');
            paramss[d[0]] = d.Length == 2 ? d[1] : "";
        }

        Params = new ReadOnlyDictionary<string, string>(paramss);
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