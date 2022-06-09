using System.Text.Json;

namespace RestPanda.Tests;

public class Tests
{
    private static readonly HttpClient Client = new ();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Login()
    {
        var result = await Client.GetAsync("http://localhost:8888/auth/?login=yes&password=no");
        Assert.That(result.IsSuccessStatusCode, Is.True);
    }
    
    [Test]
    public async Task Registration()
    {
        var content = new StringContent(JsonSerializer.Serialize(new {Login = "yes", Password = "no"}));
        var result = await Client.PostAsync("http://localhost:8888/auth/signup", content);
        Assert.That(result.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task TimeOut()
    {
        var result = await Client.GetAsync("http://localhost:8888/hello/world");
        Assert.That(result.IsSuccessStatusCode, Is.False);
    }
}