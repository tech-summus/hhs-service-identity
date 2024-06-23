using System.Security.Claims;
using Hhs.IdentityService.Domain.AppRoleDomain.Entities;
using Hhs.IdentityService.Domain.AppRoleDomain.Exceptions;
using Hhs.IdentityService.Domain.AppUserDomain.Entities;
using Hhs.IdentityService.Domain.AppUserDomain.Exceptions;
using Hhs.IdentityService.EntityFrameworkCore.Context;
using Hhs.IdentityService.EntityFrameworkCore.Setup;
using Hhs.Shared.Helper.Utils;
using HsnSoft.Base.Data;
using HsnSoft.Base.Logging;
using Microsoft.AspNetCore.Identity;
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
        var appDbContext = scope.ServiceProvider.GetRequiredService<IdentityAppDbContext>();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityServiceDbContext>();
        try
        {
            if (appDbContext.Database.CanConnectAsync(cancellationToken).GetAwaiter().GetResult())
            {
                if ((await appDbContext.Database.GetPendingMigrationsAsync(cancellationToken: cancellationToken)).Any())
                {
                    // apply pending migrations
                    await appDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
                    logger.LogDebug("EfCoreSeeder | PENDING MIGRATIONS SUCCESSFULLY APPLIED (APP)");
                }
                else
                {
                    logger.LogDebug("EfCoreSeeder | EVERYTHING IS UP TO DATE (APP)");
                }

                if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken: cancellationToken)).Any())
                {
                    // apply pending migrations
                    await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
                    logger.LogDebug("EfCoreSeeder | PENDING MIGRATIONS SUCCESSFULLY APPLIED (SERVICE)");
                }
                else
                {
                    logger.LogDebug("EfCoreSeeder | EVERYTHING IS UP TO DATE (SERVICE)");
                }
            }
            else
            {
                // first creation
                await appDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
                await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
                logger.LogDebug("EfCoreSeeder | INITIALIZE SUCCESSFULLY COMPLETED");
            }

            isReadyDatabase = true;
        }
        catch (Exception e)
        {
            logger.LogError("EfCoreSeeder | INIT ERROR: {Error}", e.Message);
        }

        if (isReadyDatabase)
        {
            try
            {
                if (!appDbContext.Roles.Any())
                {
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

                    foreach (var seedRole in SeedRoles.Roles)
                    {
                        var draftAppRole = new AppRole(
                            id: seedRole.RoleId,
                            name: seedRole.Name,
                            isDefault: seedRole.IsDefault,
                            isStatic: seedRole.IsStatic,
                            isPublic: seedRole.IsPublic,
                            tenantId: seedRole.TenantId,
                            tenantDomain: seedRole.TenantDomain
                        );

                        var result = await roleManager.CreateAsync(draftAppRole);
                        if (!result.Succeeded) throw new AppRoleIdentityException(result.Errors);

                        await roleManager.AddClaimAsync(draftAppRole, new Claim("role_name",
                            StringOperations.FirstCharCapitalize(seedRole.Name, new[] { "-", "_" })));

                        logger.LogDebug("EfCoreSeeder | SEED ROLE -> {RoleName} added", seedRole.Name);
                    }
                }

                if (!appDbContext.Users.Any())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                    foreach (var seedUser in SeedUsers.Users)
                    {
                        var draftAppUser = new AppUser(
                            tenantId: seedUser.TenantId,
                            tenantDomain: seedUser.TenantDomain,
                            id: seedUser.UserId,
                            userName: seedUser.Username,
                            email: seedUser.Email,
                            phone: seedUser.Phone,
                            name: seedUser.GivenName,
                            surname: seedUser.FamilyName,
                            defaultLanguage: "en",
                            avatarSuffixUrl: "/images/no-image.webp"
                        );

                        var result = string.IsNullOrWhiteSpace(seedUser.PlainPassword)
                            ? await userManager.CreateAsync(draftAppUser)
                            : await userManager.CreateAsync(draftAppUser, seedUser.PlainPassword);

                        if (!result.Succeeded) throw new AppUserIdentityException(result.Errors);

                        // add user roles
                        if (seedUser.Roles is { Count: > 0 })
                        {
                            await userManager.AddToRolesAsync(draftAppUser, seedUser.Roles);
                        }

                        logger.LogDebug("EfCoreSeeder | SEED USER -> {UserName} added", seedUser.Username);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError("EfCoreSeeder | SEED ERROR: {Error}", e.Message);
            }
        }
    }
}