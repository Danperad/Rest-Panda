using System.Net;

namespace RestPanda;

public class Configuration
{
    /// <summary>
    /// ContentType of Response
    /// </summary>
    public string? ContentType { get; set; }
    
    /// <summary>
    /// Basic response headers
    /// </summary>
    public Dictionary<HttpResponseHeader, string> Headers { get; } = new();

    /// <summary>
    /// Your response headers
    /// </summary>
    public Dictionary<string, string> CustomHeaders { get; } = new();

    /// <summary>
    /// Add a new title or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    public void AddHeader(HttpResponseHeader key, string value)
    {
        Headers[key] = value;
    }
    /// <summary>
    /// Add a new title or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    public void AddHeader(string key, string value)
    {
        CustomHeaders[key] = value;
    }
}