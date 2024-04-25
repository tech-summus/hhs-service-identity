using Hhs.IdentityService.Domain.FakeDomain.Repositories;
using Hhs.IdentityService.Domain.FakeDomain.Services;
using Hhs.IdentityService.EntityFrameworkCore;
using Hhs.IdentityService.FakeDomain;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hhs.IdentityService;

public static class IdentityServiceEntityFrameworkExtensions
{
    public static IServiceCollection AddServiceDatabaseConfiguration(this IServiceCollection services, [CanBeNull] string assemblyName, [CanBeNull] string connectionString)
    {
        services.AddDbContext<IdentityServiceDbContext>(options =>
        {
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
                sqlOptions.MigrationsAssembly(assemblyName);
            });
        });

        services.AddSingleton<FakeManager>();

        services.AddScoped<IFakeReadOnlyRepository, EfCoreFakeRepository>();
        services.AddScoped<IFakeManagerRepository, EfCoreFakeRepository>();

        return services;
    }
}