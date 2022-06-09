using System.Net;

namespace RestPanda;

/// <summary>
/// The configuration for server 
/// </summary>
public class PandaConfig
{
    /// <summary>
    /// Initialize a new instance of the PandaConfig class.
    /// </summary>
    public PandaConfig()
    {
        Headers = new Dictionary<HttpResponseHeader, string>();
        CustomHeaders = new Dictionary<string, string>();
        Headers[HttpResponseHeader.Server] = "PandaServer";
        MaximumConnectionCount = ProcessorThreadCount;
        TimeOut = 5000;
    }
    
    private static int ProcessorThreadCount
    {
        get
        {
            var threadCount = Environment.ProcessorCount >> 1;
            return threadCount < 1 ? 1 : threadCount;
        }
    }
    
    /// <summary>
    /// Maximum number of concurrent requests
    /// </summary>
    public int MaximumConnectionCount { get; set; }
    
    /// <summary>
    /// Basic response headers
    /// </summary>
    public Dictionary<HttpResponseHeader, string> Headers { get; }

    /// <summary>
    /// Custom response headers
    /// </summary>
    public Dictionary<string, string> CustomHeaders { get; }

    /// <summary>
    /// Add a new header or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    public void AddHeader(HttpResponseHeader key, string value)
    {
        if (key == HttpResponseHeader.Server) return;
        Headers[key] = value;
    }
    
    /// <summary>
    /// Add a new header or replace an existing one
    /// </summary>
    /// <param name="key">Header</param>
    /// <param name="value"></param>
    public void AddHeader(string key, string value)
    {
        if (key == "Server") return;
        CustomHeaders[key] = value;
    }

    internal void FixHeader()
    {
        Headers[HttpResponseHeader.Server] = "PandaServer";
    }
    
    /// <summary>
    /// Time to Force Completion of a Request (ms)
    /// </summary>
    public uint TimeOut { get; set; }
}