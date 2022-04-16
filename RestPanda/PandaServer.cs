using System.Net;
using System.Reflection;
using RestPanda.Exceptions;
using RestPanda.Requests;
using RestPanda.Requests.Attributes;

namespace RestPanda;

public sealed class PandaServer : IDisposable
{
    #region Props

    private readonly HttpListener _listener;
    private readonly IList<Uri> _urisList;
    private readonly PandaConfig _pandaConfig;

    private readonly Dictionary<(string, string), MethodInfo> _methods = new();

    private readonly Dictionary<HttpStatusCode, MethodInfo> _errors = new();

    #endregion

    #region Constructors

    public PandaServer(Type caller, params Uri[] urls) : this(new PandaConfig(), caller, urls)
    {
    }

    public PandaServer(PandaConfig pandaConfig, Type caller, params Uri[] urls) : this(pandaConfig, caller,
        urls.ToList())
    {
    }

    public PandaServer(PandaConfig pandaConfig, Type caller, IList<Uri> urls)
    {
        _listener = new HttpListener();
        _urisList = urls;
        if (_urisList.Count == 0) throw new EmptyUrlsException();
        _pandaConfig = pandaConfig;
        FindAllHandlers(caller);
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

    private void Initialize()
    {
        if (_listener.Prefixes.Count != 0) return;
        
        foreach (var uri in _urisList)
            _listener.Prefixes.Add($"{uri.Scheme}://{uri.Host}:{uri.Port}/");
    }

    /// <summary>
    /// Start listen server
    /// </summary>
    public void Start()
    {
        if (_listener.IsListening) return;
        Initialize();
        
        _listener.Start();
        Task.Run(() =>
        {
            var semaphore = new Semaphore(_pandaConfig.MaximumConnectionCount, _pandaConfig.MaximumConnectionCount);
            while (_listener.IsListening)
            {
                semaphore.WaitOne();

                _listener.GetContextAsync().ContinueWith(async (contextTask) =>
                {
                    semaphore.Release();
                    var context = await contextTask.ConfigureAwait(false);
                    var request = context.Request;
                    var response = context.Response;
                    ConfigResponse(ref response);
                    await FindHandler(request, response).ConfigureAwait(false);
                });
            }
        });
    }

    /// <summary>
    /// Stop listen server
    /// </summary>
    public void Stop()
    {
        if (!_listener.IsListening) return;
        _listener.Stop();
    }

    /// <summary>
    /// Additional customization of each response
    /// </summary>
    /// <param name="response"></param>
    private void ConfigResponse(ref HttpListenerResponse response)
    {
        _pandaConfig.FixHeader();
        foreach (var (key, value) in _pandaConfig.Headers)
        {
            response.Headers.Remove(key);
            response.Headers.Add(key, value);
        }

        foreach (var (key, value) in _pandaConfig.CustomHeaders)
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
    private Task FindHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        var rawUrl = request.RawUrl;
        var requestHttpMethod = request.HttpMethod;

        var newResponse = new PandaResponse(response);
        PandaRequest newRequest;
        if (rawUrl is null)
        {
            MainError.NotFound(newResponse);
            return Task.CompletedTask;
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
            return Task.CompletedTask;
        }

        if (_errors.TryGetValue(HttpStatusCode.NotFound, out method))
        {
            method.Invoke(null, new object?[] {newRequest, newResponse});
            return Task.CompletedTask;
        }

        MainError.NotFound(newResponse);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_listener.IsListening) Stop();
    }
}