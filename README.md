# Rest-Panda

Simple RestAPI Server

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
using System.Text;
using RestPanda.Requests;
using RestPanda.Requests.Attributes;

namespace RestPanda.Sample;

[RequestHandlerPath("/auth")]
public class GetString : RequestHandler
{
    [Get("/getstr")]
    public void GetUser()
    {
        var builder = new StringBuilder("{\"user\": \"");
        builder.Append(Params["user"]).Append("\"}");
        AddHeader("Time", DateTime.Now.ToString("O"));
        Send(builder.ToString());
    }
    
    [Post("/getstr")]
    public void SetUser()
    {
        AddHeader("Time", DateTime.Now.ToString("O"));
        var user = Bind<User>();
        if (user?.Name != null) Send(user.Name);
        Send("");
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

{
  "user": "Ivan"
}
```
###### POST
``` 
HTTP/1.1 200 OK
Content-Length: 5
Content-Type: application/json
Server: Microsoft-HTTPAPI/2.0
Time: 2022-03-27T20:00:51.2165526+03:00
Date: Sun, 27 Mar 2022 17:00:51 GMT

Petya
```