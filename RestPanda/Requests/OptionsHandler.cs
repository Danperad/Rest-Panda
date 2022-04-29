namespace RestPanda.Requests;

internal static class OptionsHandler
{
    internal static void Option(PandaRequest request, PandaResponse response)
    {
        response.Response.StatusCode = 200;
        response.Send("");
    }
}