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