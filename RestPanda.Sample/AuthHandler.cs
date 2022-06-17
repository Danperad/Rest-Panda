using System.Web;
using RestPanda.Requests;
using RestPanda.Requests.Attributes;

namespace RestPanda.Sample;

[RequestHandlerPath("/auth")]
public class AuthHandler : RequestHandler
{
    [Get("/signin")]
    [ResponseTimeout(100000)]
    public void GetUser()
    {
        var login = GetParams<string>("login");
        var password = GetParams<string>("password");
        login = HttpUtility.UrlDecode(login);
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