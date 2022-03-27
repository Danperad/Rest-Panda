using System.Net;
using System.Text;

namespace RestPanda.Requests;

/// <summary>
/// Response class
/// </summary>
public class PandaResponse
{
    internal HttpListenerResponse Response { get; }

    /// <summary>
    /// Main response ctor
    /// </summary>
    /// <param name="response">Http response</param>
    internal PandaResponse(HttpListenerResponse response)
    {
        Response = response;
    }
    
    /// <summary>
    /// Sending an empty response
    /// </summary>
    public void Send()
    {
        var buffer = Encoding.UTF8.GetBytes("");
        Response.ContentLength64 = buffer.Length;
        var output = Response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }

    /// <summary>
    /// Sending a response
    /// </summary>
    /// <param name="response">Response Body</param>
    public void Send(string response)
    {
        var buffer = Encoding.UTF8.GetBytes(response);
        Response.ContentLength64 = buffer.Length;
        var output = Response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }
    
    /// <summary>
    /// Add a new title or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    public void AddHeader(HttpResponseHeader key, string value)
    {
        Response.Headers[key] = value;
    }
    /// <summary>
    /// Add a new title or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    public void AddHeader(string key, string value)
    {
        Response.AddHeader(key, value);
    }
}