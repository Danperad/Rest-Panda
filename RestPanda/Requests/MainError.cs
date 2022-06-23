namespace RestPanda.Requests;

internal static class MainError
{
    /// <summary>
    /// Reply if there is no matching request
    /// </summary>
    /// <param name="response"></param>
    internal static void NotFound(PandaResponse response)
    {
        response.SetStatusCode(404);
        response.SetContentType("application/json");
        response.Send("{\"error\": \"not found\"}");
    }
    
    internal static void Timeout(PandaResponse response)
    {
        response.SetStatusCode(504);
        response.SetContentType("application/json");
        response.Send("{\"error\": \"server don't answer\"}");
    }
}