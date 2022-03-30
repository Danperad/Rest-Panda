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
        response.Send("{\"error\": \"not found\"}");
    }
}