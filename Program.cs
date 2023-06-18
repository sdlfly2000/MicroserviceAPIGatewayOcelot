using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

public class MicroserviceAPIGatewayOcelot
{
    public static void Main(string[] args)
    {
        new WebHostBuilder()
            .UseKestrel(options =>
            {
                // Create Self Cert: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs
                // dotnet dev-certs https -ep ./MicroserviceAPIGatewayOcelot.pfx -p sdl@1215 --trust
                options.Listen(IPAddress.Loopback, 5010, options => options.UseHttps("MicroserviceAPIGatewayOcelot.pfx","sdl@1215"));
            })
            .UseUrls("https://localhost:5010")
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("ocelot.json", true, true)
                .AddEnvironmentVariables();
            })
            .ConfigureServices(s => {
                s.AddOcelot();
                s.AddW3CLogging(options => options.FileName = "logfile.log");
            })
            .UseIISIntegration()
            .Configure(app =>
            {             
                app.UseOcelot().Wait();
            })
            .Build()
            .Run();
    }
}