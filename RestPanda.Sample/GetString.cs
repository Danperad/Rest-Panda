using System.Text;
using RestPanda.Requests;
using RestPanda.Requests.Attributes;

namespace RestPanda.Sample;

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
        var user = request.GetObject<User>();
        if (user?.Name != null) response.Send(user.Name);
        response.Send("");
    }
}