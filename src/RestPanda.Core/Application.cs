using System.Reflection;
using RestPanda.Core.Attributes;
using RestPanda.Core.Configuration;
using RestPanda.Core.Utils;

namespace RestPanda.Core;

public abstract class Application
{
    protected Application()
    {
    }

    internal static Configuration.Configuration Configuration { get; private set; }

    protected static void Launch()
    {
        var config = new DefaultConfiguration();
        Launch(config);
    }

    protected static void Launch(Configuration.Configuration conf)
    {
        Configuration = conf;
        var protocol = Configuration.Protocol == Core.Configuration.Configuration.HttpProtocol.Http ? "http" : "https";
        var port = Configuration.Port;
        var addresses = Configuration.Urls.Select(src => $"{protocol}://{src}:{port}/").ToList();
        var start = GetStartClass();
        using var server = new HttpServer(addresses, GetHandlers());
        server.Start();
        start.OnStartUp();
        Console.WriteLine("Server Started");
        Console.WriteLine("Listened on:");
        foreach (var src in addresses)
        {
            Console.WriteLine(src.Any(s => s is '*' or '+') ? $"{protocol}://0.0.0.0:{port}/" : src);
        }
        Console.WriteLine("Press Ctrl + Z to Stop");
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey();
        } while (!(key.Key == ConsoleKey.Z && key.Modifiers == ConsoleModifiers.Control));

        start.OnShutDown();
    }

    private static Application GetStartClass()
    {
        var start = Assembly.GetEntryAssembly()!.GetTypes()
            .FirstOrDefault(t => t.BaseType != null && t.BaseType == typeof(Application));
        if (start is null) throw new Exception();
        var r = (Application?) Activator.CreateInstance(start);
        if (r is null) throw new Exception();
        return r;
    }

    private static MethodsSet GetHandlers()
    {
        var tmp = new MethodsSet();
        var types = Assembly.GetEntryAssembly()!.GetTypes()
            .Where(t => t.GetCustomAttribute<ControllerAttribute>() is not null);
        foreach (var type in types)
        {
            var inst = Activator.CreateInstance(type);
            if (inst is null) throw new NotImplementedException();
            var path = type.GetCustomAttribute<ControllerAttribute>()!.Path;
            foreach (var methodInfo in type.GetMethods()
                         .Where(m => m.GetCustomAttribute<GetHandlerAttribute>() is not null))
            {
                var path2 = methodInfo.GetCustomAttribute<GetHandlerAttribute>()!.Path;
                var resPath = path + path2;
                if (tmp.Contains(resPath, "GET")) throw new NotImplementedException();
                tmp.Add(new Method(resPath, "GET", methodInfo, inst));
            }

            foreach (var methodInfo in type.GetMethods()
                         .Where(m => m.GetCustomAttribute<PostHandlerAttribute>() is not null))
            {
                var path2 = methodInfo.GetCustomAttribute<PostHandlerAttribute>()!.Path;
                var resPath = path + path2;
                if (tmp.Contains(resPath, "POST")) throw new NotImplementedException();
                tmp.Add(new Method(resPath, "GET", methodInfo, inst));
            }
        }

        if (tmp.IsEmpty) throw new NotImplementedException();
        return tmp;
    }

    protected virtual void OnStartUp()
    {
    }

    protected virtual void OnShutDown()
    {
    }
}