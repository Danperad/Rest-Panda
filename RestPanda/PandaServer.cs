using System.Net;
using System.Reflection;
using RestPanda.Exceptions;
using RestPanda.Requests;
using RestPanda.Requests.Attributes;

namespace RestPanda;

public class PandaServer
{
    #region Props

    private readonly HttpListener _listener;
    private readonly Configuration? _configuration;

    private bool _isListen = false;
    
    private readonly Dictionary<(string, string), MethodInfo> _methods = new();

    private readonly Dictionary<HttpStatusCode, MethodInfo> _errors = new();

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
        foreach (var type in caller.Assembly.GetTypes())
        {
            var handler = type.GetCustomAttribute(typeof(RequestHandler), false);
            if (handler is null)
            {
                handler = type.GetCustomAttribute(typeof(ErrorHandler), false);
                if (handler is null) continue;
                foreach (var method in type.GetMethods())
                {
                    var methodAtr = method.GetCustomAttribute(typeof(Error));
                    if (methodAtr is null) continue;
                    _errors[((Error) methodAtr).Code] = method;
                }
                continue;
            }
            var mainPath = ((RequestHandler) handler).Path;
            foreach (var method in type.GetMethods())
            {
                var methodAtr = method.GetCustomAttribute(typeof(Get));
                if (methodAtr is not null)
                    _methods[("GET", mainPath + ((Get) methodAtr).Path)] = method;
                else if ((methodAtr = method.GetCustomAttribute(typeof(Post))) is not null)
                    _methods[("POST", mainPath + ((Post) methodAtr).Path)] = method;
            }
        }

        if (_methods.Count == 0) throw new NoHandlersException("Handlers not found");
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
            MainError.NotFound(newResponse);
            return;
        }
        rawUrl = rawUrl.Remove(0, 1);
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

        if (_methods.TryGetValue((requestHttpMethod, path), out var method))
        {
            method.Invoke(null, new object?[] {newRequest, newResponse});
            return;
        }

        if (_errors.TryGetValue(HttpStatusCode.NotFound, out method))
        {
            method.Invoke(null, new object?[] {newRequest, newResponse});
            return;
        }
        MainError.NotFound(newResponse);
    }
}