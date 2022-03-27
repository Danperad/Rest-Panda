using System.Net;
using System.Text;

namespace RestPanda.Requests;

public class PandaResponse
{
    internal HttpListenerResponse Response { get; }

    internal PandaResponse(HttpListenerResponse response)
    {
        Response = response;
    }

    public void Send(string response)
    {
        var buffer = Encoding.UTF8.GetBytes(response);
        Response.ContentLength64 = buffer.Length;
        var output = Response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }
}