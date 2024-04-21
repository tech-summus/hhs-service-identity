using Hhs.IdentityService.Application.Contracts.FakeDomain.Interfaces;
using Hhs.IdentityService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hhs.IdentityService.Application;

public static class IdentityServiceApplicationExtensions
{
    public static IServiceCollection AddServiceApplicationConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(IdentityServiceApplicationAutoMapperProfile));

        services.AddTransient<IFakeAppService, FakeAppService>();

        return services;
    }
}