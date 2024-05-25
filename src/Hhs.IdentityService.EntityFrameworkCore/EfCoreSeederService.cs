using Hhs.IdentityService.EntityFrameworkCore.Context;
using HsnSoft.Base.Data;
using HsnSoft.Base.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        var logger = scope.ServiceProvider.GetRequiredService<IBaseLogger>();
        logger.LogDebug("EfCoreSeeder | START");

        var isReadyDatabase = false;
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityServiceDbContext>();
        try
        {
            if (dbContext.Database.CanConnectAsync(cancellationToken).GetAwaiter().GetResult())
            {
                if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken: cancellationToken)).Any())
                {
                    // apply pending migrations
                    await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
                    logger.LogDebug("EfCoreSeeder | PENDING MIGRATIONS SUCCESSFULLY APPLIED");
                }
                else
                {
                    logger.LogDebug("EfCoreSeeder | EVERYTHING IS UP TO DATE");
                }
            }
            else
            {
                // first creation
                await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
                logger.LogDebug("EfCoreSeeder | INITIALIZE SUCCESSFULLY COMPLETED");
            }

            isReadyDatabase = true;
        }
        catch (Exception e)
        {
            logger.LogError("EfCoreSeeder | FAIL: {Error}", e.Message);
        }
    }
}