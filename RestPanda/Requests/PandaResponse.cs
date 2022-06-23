using System.Net;
using System.Text;
using System.Text.Json;

namespace RestPanda.Requests;

/// <summary>
/// Response class
/// </summary>
internal class PandaResponse
{
    internal HttpListenerResponse Response { get; }

    /// <summary>
    /// Request execution status.
    /// </summary>
    internal bool IsComplete { get; private set; }

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
        if (IsComplete) return;
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
        catch
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
        try
        {
            Response.ContentLength64 = buffer.Length;
            var output = Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            Response.Close();
        }
        finally 
        {
            IsComplete = true;
        }
        
    }

    /// <summary>
    /// Add a new title or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    internal void AddHeader(HttpResponseHeader key, string value)
    {
        if (key == HttpResponseHeader.Server) return;
        try
        {
            Response.Headers[key] = value;
        }
        catch 
        {
            // ignored
        }
    }
    /// <summary>
    /// Add a new title or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    internal void AddHeader(string key, string value)
    {
        if (key == "Server") return;
        try
        {
            Response.AddHeader(key, value);
        }
        catch 
        {
            // ignored
        }
    }
    
    internal void SetContentType(string value)
    {
        try
        {
            Response.ContentType = value;
        }
        catch
        {
            // ignored
        }
    }

    internal void SetStatusCode(int code)
    {
        try
        {
            Response.StatusCode = code;
        }
        catch
        {
            // ignored
        }
    }
}