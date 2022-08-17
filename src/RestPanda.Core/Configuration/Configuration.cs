using RestPanda.Core.Utils;

namespace RestPanda.Core.Configuration;

public abstract class Configuration
{
    public enum HttpProtocol
    {
        Http, Https
    }
    public HttpProtocol Protocol { get; protected set; }
    public IEnumerable<string> Urls { get; protected set; }
    public short Port{ get; protected set; }
    public double TimeOut { get; protected set; } 
    public Configuration()
    {
        Urls = new List<string> {"+"};
        Port = 8080;
        Protocol = HttpProtocol.Http;
    }
}