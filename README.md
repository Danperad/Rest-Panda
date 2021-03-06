# Rest-Panda

Simple HTTP Server

## Installation

```
> dotnet add package RestPanda
```

## Examples

#### Main
```csharp
using RestPanda;

var config = new PandaConfig();

var urls = new List<Uri>(new []{new Uri("http://localhost:8888/"),new Uri("http://127.0.0.1:8889/")});
using var server = new PandaServer(config, urls);

server.Start();
Console.Read();
server.Stop();
```

#### GetString
```csharp
using RestPanda.Requests;
using RestPanda.Requests.Attributes;

namespace RestPanda.Sample;

[RequestHandlerPath("/auth")]
public class AuthHandler : RequestHandler
{
    [Get("/signin")]
    public void GetUser()
    {
        var login = GetParams<string>("login");
        var password = GetParams<string>("password");
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            Send(false.ToString());
            return;
        }
        Send(true.ToString());
    }

    [Post("/signup")]
    public void SetUser()
    {
        var user = Bind<User>();
        AddHeader("Time", DateTime.Now.ToString("O"));
        if (user is not null) Send(user);
    }

    [Get("/time")]
    [ResponseTimeout(1000)]
    public async Task HeHeLoisIAmLongTask()
    {
        var r = await Task.Run(() =>
        {
            Thread.Sleep(2000);
            return true;
        });
        Send(r);
    }
}
```

#### Answer
###### GET
``` 
HTTP/1.1 200 OK
Content-Length: 16
Content-Type: application/json
Server: Microsoft-HTTPAPI/2.0
Time: 2022-03-27T19:53:02.2694606+03:00
Date: Sun, 27 Mar 2022 16:53:02 GMT

true
```
###### POST
``` 
HTTP/1.1 200 OK
Content-Length: 5
Content-Type: application/json
Server: Microsoft-HTTPAPI/2.0
Time: 2022-03-27T20:00:51.2165526+03:00
Date: Sun, 27 Mar 2022 17:00:51 GMT

{
    Login: Yes,
    Password: No
}
```