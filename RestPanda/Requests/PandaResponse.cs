using System.Net;
using System.Text;
using System.Text.Json;

namespace RestPanda.Requests;

/// <summary>
/// Response class
/// </summary>
public class PandaResponse
{
    internal HttpListenerResponse Response { get; }
    private bool _isComplete = false;

    public Stream OutputStream => Response.OutputStream;

    /// <summary>
    /// Main response ctor
    /// </summary>
    /// <param name="response">Http response</param>
    internal PandaResponse(HttpListenerResponse response)
    {
        Response = response;
    }
    
    /// <summary>
    /// Sending object (Serialized to json)
    /// </summary>
    /// <param name="obj"></param>
    public void Send(object obj)
    {
        var result = JsonSerializer.Serialize(obj);
        SetContentType("application/json");
        Send(result);
    }

    /// <summary>
    /// Sending a response
    /// </summary>
    /// <param name="response">Response Body</param>
    public void Send(string response)
    { 
        if (_isComplete) return;
        var buffer = Encoding.UTF8.GetBytes(response);
        Response.ContentLength64 = buffer.Length;
        var output = Response.OutputStream;
        _isComplete = true;
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
        if (key == HttpResponseHeader.Server) return;
        Response.Headers[key] = value;
    }
    /// <summary>
    /// Add a new title or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    public void AddHeader(string key, string value)
    {
        if (key == "Server") return;
        Response.AddHeader(key, value);
    }
    
    public void SetContentType(string value)
    {
        Response.ContentType = value;
    }
}