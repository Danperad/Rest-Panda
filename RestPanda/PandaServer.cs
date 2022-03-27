using System.Net;
using RestPanda.Requests;

namespace RestPanda;

public class PandaServer
{
    private readonly string _uri;
    private readonly HttpListener _listener;
    private readonly Configuration? _configuration;

    private bool _isListen = false;

    private readonly Dictionary<string, Type> _types = new();

    public PandaServer(string url, Type caller)
    {
        _uri = url;
        _listener = new HttpListener();
        _listener.Prefixes.Add(_uri);
        FindAllHandlers(caller);
    }

    public PandaServer(string url, Configuration configuration, Type caller) : this(url, caller)
    {
        _configuration = configuration;
    }
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
    }
    
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
            await FindHandler(request, response);
        }
    }

    public void Stop()
    {
        if (!_listener.IsListening) return;

        _listener.Stop();
        _isListen = false;
    }

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

    private Task FindHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        var rawUrl = request.RawUrl;
        var requestHttpMethod = request.HttpMethod;
        
        var newResponse = new PandaResponse(response);
        PandaRequest newRequest;
        if (rawUrl is null)
        {
            Error.NotFound(newResponse);
            return Task.CompletedTask;
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
                return Task.CompletedTask;
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
            return Task.CompletedTask;
        }
        
        foreach (var method in _types[path].GetMethods())
        {
            var ss = method.GetCustomAttributes(httpMethod, false);
            if (ss.Length <= 0) continue;
                
            var obj = ((IRequest) ss[0]).Path;
            if (obj != req) continue;
                
            method.Invoke(null, new object?[]{newRequest, newResponse});
            return Task.CompletedTask;
        }
        
        Error.NotFound(newResponse);
        return Task.CompletedTask;
    }
}