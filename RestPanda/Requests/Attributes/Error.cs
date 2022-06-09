using System.Net;

namespace RestPanda.Requests.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class Error : Attribute
{
    public HttpStatusCode Code { get; }

    public Error(HttpStatusCode code)
    {
        Code = code;
    }
}