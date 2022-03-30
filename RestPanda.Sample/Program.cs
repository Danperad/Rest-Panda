using RestPanda;

var config = new Configuration
{
    ContentType = "application/json"
};
var urls = new List<string>(new []{"http://localhost:8888/", "http://127.0.0.1:8889/"});
var server = new PandaServer(urls, config, typeof(Program));
server.Start();
Console.Read();
server.Stop();