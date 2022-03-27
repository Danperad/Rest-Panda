using System.Net;

namespace RestPanda;

public class Configuration
{
    public string? ContentType { get; set; }
    public Dictionary<HttpResponseHeader, string> Headers { get; } = new();

    public Dictionary<string, string> CustomHeaders { get; } = new();

    public void AddHeader(HttpResponseHeader key, string value)
    {
        Headers[key] = value;
    }
    public void AddHeader(string key, string value)
    {
        CustomHeaders[key] = value;
    }
}