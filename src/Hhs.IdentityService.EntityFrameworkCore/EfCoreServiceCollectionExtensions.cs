using Hhs.IdentityService.Domain.FakeDomain.Repositories;
using Hhs.IdentityService.EntityFrameworkCore.Context;
using Hhs.IdentityService.EntityFrameworkCore.Repositories;
using HsnSoft.Base.Auditing;
using HsnSoft.Base.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hhs.IdentityService.EntityFrameworkCore;

public static class EfCoreServiceCollectionExtensions
{
    public static IServiceCollection AddServiceDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBaseAuditingServiceCollection();
        services.AddBaseDataServiceCollection();

        // override DefaultBasicDataSeeder
        services.AddTransient<IBasicDataSeeder, EfCoreSeederService>();

        services.AddDbContext<IdentityServiceDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(EfCoreDbProperties.ConnectionStringName), sqlOptions =>
                {
                    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
                    sqlOptions.MigrationsAssembly(typeof(IdentityServiceDbContext).Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(6), null);
                    sqlOptions.CommandTimeout(30000);
                    sqlOptions.MaxBatchSize(100);
                });
                // options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                options.EnableSensitiveDataLogging(false);
            }
            , contextLifetime: ServiceLifetime.Scoped   // Must be Scoped => Cannot consume any scoped service and CurrentUser object creation on constructor
            , optionsLifetime: ServiceLifetime.Singleton
        );

        // Must be Scoped => Cannot consume any scoped service and CurrentUser object creation on constructor
        services.AddScoped<IFakeRepository, EfCoreFakeRepository>();

        return services;
    }
}