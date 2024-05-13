using Hhs.IdentityService.Application.Contracts.FakeDomain;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Interfaces;
using Hhs.IdentityService.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hhs.IdentityService.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddServiceApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(ApplicationAutoMapperProfile));

        services.Configure<FakeSettings>(configuration.GetSection("FakeSettings"));

        // Must be Scoped or Transient => Cannot consume any scoped service
        services.AddScoped<IFakeAppService, FakeAppService>();

        return services;
    }
}