namespace RestPanda.Requests;

internal static class MainError
{
    /// <summary>
    /// Reply if there is no matching request
    /// </summary>
    /// <param name="response"></param>
    internal static void NotFound(PandaResponse response)
    {
        response.Response.StatusCode = 404;
        response.Response.ContentType = "application/json";
        response.Send("{\"error\": \"not found\"}");
    }
    
    internal static void Timeout(PandaResponse response)
    {
        response.Response.StatusCode = 504;
        response.Response.ContentType = "application/json";
        response.Send("{\"error\": \"server don't answer\"}");
    }
}