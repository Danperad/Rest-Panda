using RestPanda;

var config = new PandaConfig();

var urls = new List<Uri>(new []{new Uri("http://localhost:8888/"),new Uri("http://127.0.0.1:8889/")});
using var server = new PandaServer(config, urls);

server.Start();
Console.Read();
server.Stop();