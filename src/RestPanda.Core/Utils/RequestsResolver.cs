using System.Net;

namespace RestPanda.Core.Utils;

internal class RequestsResolver
{
    private HashSet<HttpListenerContext> _requests;
    private readonly MethodsSet _handlers;
    internal RequestsResolver(MethodsSet handlers)
    {
        _requests = new HashSet<HttpListenerContext>();
        _handlers = handlers;
    }

    internal void AddRequest(HttpListenerContext context)
    {
        _requests.Add(context);
        Task.Run(() => PrepareRequest(context));
    }

    private void RemoveRequest(HttpListenerContext item)
    {
        _requests.Remove(item);
    }

    private void PrepareRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var path = request.Url;
        if (path is null) throw new NotImplementedException();
        var mainPath = path.LocalPath;
        if (!_handlers.Contains(mainPath, request.HttpMethod))
        {
            throw new NotImplementedException();
        }
        var query = GetStringQuery(path.Query);
        RemoveRequest(context);
    }

    private static Dictionary<string, string> GetStringQuery(string query)
    {
        var tmp = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(query)) return tmp;
        var q = query.Remove(0,1).Split("&");
        foreach (var s in q)
        {
            var tmp2 = s.Split("=");
            tmp.Add(tmp2[0], tmp2[1]);
        }

        return tmp;
    }
}