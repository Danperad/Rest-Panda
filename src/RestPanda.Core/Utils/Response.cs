using System.Net;
using System.Text;
using RestPanda.Core.Attributes;

namespace RestPanda.Core.Utils;

[Response]
public sealed class Response
{
    public WebHeaderCollection Headers { get; private set; }
    public Encoding? ContentEncoding { get; set; }
    public string? ContentType { get; set; }
    public int StatusCode { get; set; }
    public long ContentLength64 { get; set; }
    public Stream OutputStream { get; private set; }
    public string StatusDescription { get; set; }

    internal Response(HttpListenerResponse response)
    {
        OutputStream = response.OutputStream;
        Headers = response.Headers;
        ContentEncoding = response.ContentEncoding;
        ContentType = response.ContentType;
        StatusCode = response.StatusCode;
        ContentLength64 = response.ContentLength64;
        StatusDescription = response.StatusDescription;
    }
}