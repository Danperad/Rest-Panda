using System.Net;
using System.Text;
using System.Text.Json;

namespace RestPanda.Requests;

/// <summary>
/// Response class
/// </summary>
internal class PandaResponse : IDisposable
{
    internal HttpListenerResponse Response { get; }

    /// <summary>
    /// Request execution status.
    /// </summary>
    internal bool IsComplete { get; private set; } = false;

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
    public void Send(object? obj)
    {
        string result;
        if (obj is null)
        {
            Send("null");
            return;
        }
        try
        {
            result = JsonSerializer.Serialize(obj);
            SetContentType("application/json");
        }
        catch (NotSupportedException e)
        {
            result = obj.ToString() ?? "null";
        }
        Send(result);
    }

    /// <summary>
    /// Sending a response
    /// </summary>
    /// <param name="response">Response Body</param>
    public void Send(string response)
    { 
        if (IsComplete) return;
        var buffer = Encoding.UTF8.GetBytes(response);
        Response.ContentLength64 = buffer.Length;
        var output = Response.OutputStream;
        IsComplete = true;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
        Response.Close();
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
    
    private void SetContentType(string value)
    {
        Response.ContentType = value;
    }

    public void Dispose()
    {
        Response.Close();
        ((IDisposable) Response).Dispose();
    }
}