using Hhs.IdentityService.Domain.FakeDomain.Entities;
using Hhs.IdentityService.EntityFrameworkCore.Configurations;
using HsnSoft.Base.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hhs.IdentityService.EntityFrameworkCore.Context;

public sealed class IdentityServiceDbContext : BaseEfCoreDbContext<IdentityServiceDbContext>
{
    public DbSet<Fake> Fakes { get; set; }

    public IdentityServiceDbContext(IServiceProvider provider, DbContextOptions<IdentityServiceDbContext> options) : base(options, provider)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureFakeEntity();
    }
}