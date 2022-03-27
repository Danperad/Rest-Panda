using System.Net;

namespace RestPanda.Requests;

public class PandaRequest
{
    private HttpListenerRequest _request;
    public Dictionary<string, string> Params { get; } = new ();

    internal PandaRequest(HttpListenerRequest request)
    {
        _request = request;
    }
    internal PandaRequest(HttpListenerRequest request, string param) : this(request)
    {
        var s = param.Split('&');
        foreach (var keys in s)
        {
            var d = keys.Split('=');
            this.Params[d[0]] = d.Length == 2 ? d[1] : "";
        }
    }

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
}