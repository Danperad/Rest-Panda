using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using RestPanda.Requests.Attributes;
using Timer = System.Timers.Timer;

namespace RestPanda.Requests;

/// <summary>
/// Base class for request handler.
/// </summary>
public abstract class RequestHandler
{
    private PandaRequest _request = null!;

    internal PandaRequest Request
    {
        get => _request;
        set
        {
            _request = value;
            _isCompleted = false;
        }
    }

    internal PandaResponse Response { get; set; } = null!;

    private bool _isCompleted = true;

    /// <summary>
    /// Request parameters
    /// </summary>
    protected ReadOnlyDictionary<string, string> Params => _isCompleted
        ? new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())
        : Request.Params;

    /// <summary>
    /// Request headers
    /// </summary>
    protected ReadOnlyDictionary<string, string> Headers => _isCompleted
        ? new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())
        : Request.Headers;

    internal void FindHandler(string httpMethod, string path, uint timeout)
    {
        var request = httpMethod switch
        {
            "GET" => typeof(Get),
            "POST" => typeof(Post),
            _ => null
        };
        if (request is null) return;
        foreach (var method in GetType().GetMethods())
        {
            var attribute = method.GetCustomAttribute(request);
            if (attribute == null) continue;
            if (((IRequest) attribute).Path != path) continue;
            var time = method.GetCustomAttribute<ResponseTimeout>();
            if (time is not null) timeout = time.Time;
            var timer = new Timer(timeout);
            timer.Enabled = true;
            timer.Elapsed += (_, _) =>
            {
                if (!Response.IsComplete) MainError.Timeout(Response);
            };
            method.Invoke(this, null);
            while (!Response.IsComplete)
            {
            }

            timer.Dispose();
            return;
        }
    }

    #region Response

    /// <summary>
    /// Add a new header or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    protected void AddHeader(HttpResponseHeader key, string value)
    {
        Response.AddHeader(key, value);
    }

    /// <summary>
    /// Add a new header or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    protected void AddHeader(string key, string value)
    {
        Response.AddHeader(key, value);
    }

    /// <summary>
    /// Send text answer
    /// </summary>
    /// <param name="answer"></param>
    protected void Send(string answer)
    {
        if (_isCompleted) return;
        _isCompleted = true;
        Response?.Send(answer);
    }

    /// <summary>
    /// Sent json answer
    /// </summary>
    /// <param name="answer"></param>
    protected void Send(object? answer)
    {
        if (_isCompleted) return;
        _isCompleted = true;
        Response.Send(answer);
    }

    #endregion

    #region Request

    /// <summary>
    /// Is there a request body, if so, output to a variable
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public bool TryGetBody(out string body)
    {
        body = "";
        return !_isCompleted && Request.TryGetBody(out body);
    }

    /// <summary>
    /// Convert request body to T
    /// </summary>
    /// <typeparam name="T">Type to convert</typeparam>
    /// <returns>Transformed request body, if any</returns>
    /// <exception cref="NullReferenceException">Request does not exist</exception>
    protected T? Bind<T>()
    {
        if (!_isCompleted) return Request.GetObject<T>();
        throw new NullReferenceException();
    }

    /// <summary>
    /// Get Params request with different type
    /// </summary>
    /// <param name="key">Params name</param>
    /// <typeparam name="T">New type (IConvertible)</typeparam>
    /// <returns>Value or default</returns>
    protected T? GetParams<T>(string key) where T : IConvertible
    {
        if (_isCompleted) throw new NullReferenceException();
        if (!Params.TryGetValue(key, out var value)) return default;
        T? res = default;
        try
        {
            res = (T?) Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            // ignored
        }
        return res; 
    }

    #endregion
}