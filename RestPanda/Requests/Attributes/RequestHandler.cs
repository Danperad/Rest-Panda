﻿namespace RestPanda.Requests.Attributes;

/// <summary>
/// Attribute for request handler
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RequestHandler : Attribute
{
    public string Path { get; }

    /// <summary>
    /// Attribute for request handler
    /// </summary>
    /// <param name="path">The path of the requests</param>
    public RequestHandler(string path)
    {
        Path = path;
        if (Path.StartsWith("/")) Path = Path.Remove(0,1);
    }
    public RequestHandler()
    {
        Path = "";
    }
}