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
    private readonly List<Uri> _urisList;
    private readonly PandaConfig _pandaConfig;

    // private readonly Dictionary<(string, string), MethodInfo> _methods = new();

    private readonly Dictionary<string, RequestHandler> _handlers = new();

    #endregion

    #region Constructors

    public PandaServer(Uri url, params Uri[] urls) : this(new PandaConfig(), url, urls)
    {
    }

    public PandaServer(PandaConfig pandaConfig, Uri url, params Uri[] urls)
    {
        _listener = new HttpListener();
        _urisList = new List<Uri>(urls) {url};
        if (_urisList.Count == 0) throw new EmptyUrlsException();
        _pandaConfig = pandaConfig;
        FindAllHandlers();
    }

    public PandaServer(PandaConfig pandaConfig, IEnumerable<Uri> urls)
    {
        _listener = new HttpListener();
        _urisList = new List<Uri>(urls);
        if (_urisList.Count == 0) throw new EmptyUrlsException();
        _pandaConfig = pandaConfig;
        FindAllHandlers();
    }

    #endregion

    /// <summary>
    /// Search all request handler
    /// </summary>
    private void FindAllHandlers()
    {
        foreach (var type in 
                 Assembly.GetEntryAssembly()!.GetTypes()
                     .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(RequestHandler))))
        {
            var assemblyType = (RequestHandler) Activator.CreateInstance(type)!;
            var ss = assemblyType.GetType().GetCustomAttribute(typeof(RequestHandlerPath));
            if (ss is null) throw new CustomAttributeFormatException("Request Path Attribute Not Found");
            _handlers[((RequestHandlerPath) ss).Path] = assemblyType;
        }
        if (_handlers.Count == 0) throw new NoHandlersException("Handlers not found");
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

        if (requestHttpMethod == "OPTIONS")
        {
            OptionsHandler.Option(newRequest, newResponse);
            return Task.CompletedTask;
        }
        if (_handlers.TryGetValue(path[..path.LastIndexOf("/", StringComparison.Ordinal)], out var type))
        {
            type.Request = newRequest;
            type.Response = newResponse;
            type.FindHandler(requestHttpMethod, path[path.LastIndexOf('/')..]);
            if (newResponse._isComplete) return Task.CompletedTask;
        }

        MainError.NotFound(newResponse);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_listener.IsListening) Stop();
    }
}