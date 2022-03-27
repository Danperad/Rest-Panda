namespace RestPanda.Requests;

internal static class Error
{
    internal static void NotFound(PandaResponse response)
    {
        response.Response.StatusCode = 404;
        response.Send("{\"error\": \"not found\"}");
    }
}