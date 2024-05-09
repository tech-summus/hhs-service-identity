using Hhs.IdentityService.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hhs.IdentityService.EntityFrameworkCore.Factory;

internal sealed class IdentityServiceDbContextFactory : IDesignTimeDbContextFactory<IdentityServiceDbContext>
{
    public IdentityServiceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<IdentityServiceDbContext>()
            .UseNpgsql(DbContextFactoryHelper.GetConnectionStringFromConfiguration(), b =>
            {
                b.MigrationsHistoryTable("__EFMigrationsHistory");
            });

        return new IdentityServiceDbContext(null, builder.Options);
    }
}