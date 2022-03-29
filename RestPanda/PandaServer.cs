using System.Net;
using RestPanda.Exceptions;
using RestPanda.Requests;

namespace RestPanda;

public class PandaServer
{
    #region Props

    private readonly HttpListener _listener;
    private readonly Configuration? _configuration;

    private bool _isListen = false;

    private readonly Dictionary<string, Type> _types = new();

    #endregion

    #region Constructors

    public PandaServer(string url, Type caller)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(url);
        FindAllHandlers(caller);
    }
    
    public PandaServer(string url, Configuration configuration, Type caller) : this(url, caller)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Main .ctor
    /// </summary>
    /// <param name="urls">List of listening addresses</param>
    /// <param name="caller">Class from project with request handlers</param>
    public PandaServer(List<string> urls, Type caller)
    {
        if (urls.Count == 0) throw new EmptyUrlsException("Url list must be not empty");
        _listener = new HttpListener();
        foreach (var url in urls)
        {
            _listener.Prefixes.Add(url);
        }

        FindAllHandlers(caller);
    }

    /// <summary>
    /// Configuration Server .ctor
    /// </summary>
    /// <param name="urls">List of listening addresses</param>
    /// <param name="configuration">Configuration class</param>
    /// <param name="caller">Class from project with request handlers</param>    
    public PandaServer(List<string> urls, Configuration configuration, Type caller) : this(urls, caller)
    {
        _configuration = configuration;
    }

    #endregion

    /// <summary>
    /// Search all request handler
    /// </summary>
    /// <param name="caller">Class from project with request handlers</param>
    private void FindAllHandlers(Type caller)
    {
        var s = caller.Assembly.GetTypes();
        foreach (var type in s)
        {
            var ss = type.GetCustomAttributes(typeof(RequestHandler), false);
            if (ss.Length > 0)
            {
                _types[((RequestHandler) ss[0]).Path] = type;
            }
        }
        if (_types.Count == 0) throw new NoHandlersException("Handlers not found");
    }

    /// <summary>
    /// Start listen server
    /// </summary>
    public async Task Start()
    {
        _listener.Start();
        _isListen = true;
        while (_isListen)
        {
            var context = await _listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;
            if (_configuration is not null) ConfigResponse(ref response);
            FindHandler(request, response);
        }
    }

    /// <summary>
    /// Stop listen server
    /// </summary>
    public void Stop()
    {
        if (!_listener.IsListening) return;

        _listener.Stop();
        _isListen = false;
    }

    /// <summary>
    /// Additional customization of each response
    /// </summary>
    /// <param name="response"></param>
    private void ConfigResponse(ref HttpListenerResponse response)
    {
        if (_configuration is null) return;
        if (_configuration.ContentType is not null)
            response.ContentType = _configuration.ContentType;
        foreach (var (key, value) in _configuration.Headers)
        {
            response.Headers.Remove(key);
            response.Headers.Add(key, value);
        }

        foreach (var (key, value) in _configuration.CustomHeaders)
        {
            response.Headers.Remove(key);
            response.Headers.Add(key, value);
        }
    }

    /// <summary>
    /// Finding and calling the desired handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    private void FindHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        var rawUrl = request.RawUrl;
        var requestHttpMethod = request.HttpMethod;

        var newResponse = new PandaResponse(response);
        PandaRequest newRequest;
        if (rawUrl is null)
        {
            Error.NotFound(newResponse);
            return;
        }

        Type httpMethod;
        switch (requestHttpMethod)
        {
            case "GET":
                httpMethod = typeof(Get);
                break;
            case "POST":
                httpMethod = typeof(Post);
                break;
            default:
                Error.NotFound(newResponse);
                return;
        }

        string path;
        if (rawUrl.Contains('?'))
        {
            path = rawUrl[..rawUrl.IndexOf('?')];
            newRequest = new PandaRequest(request, rawUrl[rawUrl.IndexOf('?')..].Replace("?", ""));
        }
        else
        {
            path = rawUrl;
            newRequest = new PandaRequest(request);
        }

        var req = path[path.LastIndexOf("/", StringComparison.Ordinal)..];
        path = path[..path.LastIndexOf("/", StringComparison.Ordinal)];

        if (!_types.ContainsKey(path))
        {
            Error.NotFound(newResponse);
            return;
        }

        foreach (var method in _types[path].GetMethods())
        {
            var ss = method.GetCustomAttributes(httpMethod, false);
            if (ss.Length <= 0) continue;

            var obj = ((IRequest) ss[0]).Path;
            if (obj != req) continue;
            method.Invoke(null, new object?[] {newRequest, newResponse});
            return;
        }

        Error.NotFound(newResponse);
    }
}