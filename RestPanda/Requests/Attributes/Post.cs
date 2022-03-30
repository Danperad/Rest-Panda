﻿namespace RestPanda.Requests.Attributes;

/// <summary>
/// Attribute for POST requests
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Post : Attribute, IRequest
{
    public string Path { get; }

    /// <summary>
    /// Attribute for POST requests
    /// </summary>
    /// <param name="path">The path of the request</param>
    public Post(string path)
    {
        Path = path;
        if (!Path.StartsWith("/") && Path != "") Path = "/" + Path;
    }

    public Post()
    {
        Path = "";
    }
}