using Microsoft.AspNetCore;

namespace Hhs.IdentityService;

public static class Program
{
    private static readonly string Namespace = typeof(Startup).Namespace;
    private static readonly string AppName = Namespace?[(Namespace.IndexOf('.') + 1)..];

    public static async Task<int> Main(string[] args)
    {
        var configuration = GetConfiguration();

        try
        {
            var host = CreateHostBuilder(configuration, args);

            await host.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            return 1;
        }
    }

    private static IWebHost CreateHostBuilder(IConfiguration configuration, string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .CaptureStartupErrors(false)
            .ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBufferSize = long.MaxValue;
                options.Limits.MaxRequestBodySize = long.MaxValue;
            })
            .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
            .UseStartup<Startup>()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();

    private static IConfiguration GetConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
            .AddEnvironmentVariables()
            .Build();
}