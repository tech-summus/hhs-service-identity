using Hhs.Shared.Hosting;
using Microsoft.AspNetCore;
using Serilog;

namespace Hhs.IdentityService;

public static class Program
{
    private static readonly string Namespace = typeof(Startup).Namespace;
    private static readonly string AppName = Namespace?[(Namespace.IndexOf('.') + 1)..];

    public static async Task<int> Main(string[] args)
    {
        Log.Logger = SerilogConfigurationHelper.Configure(AppName);

        try
        {
            Log.Information("Configuring web host ({ApplicationContext})...", AppName);
            var host = CreateHostBuilder(GetConfiguration(), args);

            using (var scope = host.Services.CreateScope())
            {
            }

            Log.Information("Starting web host ({ApplicationContext})...", AppName);
            await host.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
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
                logging.AddSerilog(Log.Logger);
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