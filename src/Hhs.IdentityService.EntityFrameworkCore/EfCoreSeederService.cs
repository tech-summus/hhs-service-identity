using Hhs.IdentityService.EntityFrameworkCore.Context;
using HsnSoft.Base.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hhs.IdentityService.EntityFrameworkCore;

public sealed class EfCoreSeederService : IBasicDataSeeder
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EfCoreSeederService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task EnsureSeedDataAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("EfCoreSeederService");
        logger.LogInformation("SEED OPERATION | START");

        try
        {
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var connectionStr = configuration.GetConnectionString(EfCoreDbProperties.ConnectionStringName);

            var dbContext = scope.ServiceProvider.GetRequiredService<IdentityServiceDbContext>();
            dbContext.Database.SetConnectionString(connectionStr);

            if (dbContext.Database.CanConnectAsync(cancellationToken).GetAwaiter().GetResult())
            {
                if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken: cancellationToken)).Any())
                {
                    // apply pending migrations
                    await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
                    logger.LogInformation("SEED OPERATION | PENDING MIGRATIONS SUCCESSFULLY APPLIED");
                }
                else
                {
                    logger.LogInformation("SEED OPERATION | EVERYTHING IS UP TO DATE");
                }
            }
            else
            {
                // first creation
                await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
                logger.LogInformation("SEED OPERATION | FIRST INITIALIZE SUCCESSFULLY COMPLETED");
            }
        }
        catch (Exception e)
        {
            logger.LogInformation("SEED OPERATION | FAIL: {Error}", e.Message);
        }
    }
}