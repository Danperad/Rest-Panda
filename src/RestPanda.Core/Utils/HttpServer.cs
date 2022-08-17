using System.Diagnostics;
using System.Net;

namespace RestPanda.Core.Utils;

internal class HttpServer : IDisposable
{
    private readonly HttpListener _listener;
    private bool _isListen;
    private readonly RequestsResolver _resolver;
    internal HttpServer(IEnumerable<string> addresses, MethodsSet handlers)
    {
        _listener = new HttpListener();
        foreach (var address in addresses)
        {
            TryGetAny(address);
            _listener.Prefixes.Add(address);
        }

        _resolver = new RequestsResolver(handlers);
        _isListen = false;
    }
    // TODO: Rename
    private static void TryGetAny(string address)
    {
        var args = $@"http add urlacl url={address} user={Environment.UserDomainName}\{Environment.UserName}";

        var psi = new ProcessStartInfo("netsh", args)
        {
            Verb = "runas",
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = true
        };

        Process.Start(psi)?.WaitForExit();
    }
    internal async void Start()
    {
        if (_isListen) return;
        _isListen = true;
        _listener.Start();
        while (_isListen)
        {
            var context = await _listener.GetContextAsync();
            _resolver.AddRequest(context);
        }
    }

    internal void Stop()
    {
        _isListen = false;
        _listener.Stop();
    }

    public void Dispose()
    {
        Stop();
        _listener.Close();
    }
}