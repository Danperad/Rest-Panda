﻿using System;

namespace RestPanda.Requests;

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
        if (path == "/") Path = "";
        else if (!Path.StartsWith("/")) Path = "/" + Path;
    }
    public RequestHandler()
    {
        Path = "";
    }
}