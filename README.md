# Rest-Panda

## Installation

```
> dotnet add package RestPanda
```

## Examples

#### Main
```csharp
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
```

#### GetString
```csharp
using System.Text;
using System.Text.Json;
using RestPanda.Requests;

namespace AnyName.Space;

[RequestHandler("/auth")]
public class GetString
{
    [Get("/getstr")]
    public static void GetUser(PandaRequest request, PandaResponse response)
    {
        var builder = new StringBuilder("{\"user\": \"");
        builder.Append(request.Params["user"]).Append("\"}");
        response.AddHeader("Time", DateTime.Now.ToString("O"));
        response.Send(builder.ToString());
    }
    
    [Post("/getstr")]
    public static void SetUser(PandaRequest request, PandaResponse response)
    {
        response.AddHeader("Time", DateTime.Now.ToString("O"));
        if (request.TryGetBody(out var body))
        {
            var user = JsonSerializer.Deserialize<User>(body);
            if (user?.Name != null) response.Send(user.Name);
            return;
        }
        response.Send();
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