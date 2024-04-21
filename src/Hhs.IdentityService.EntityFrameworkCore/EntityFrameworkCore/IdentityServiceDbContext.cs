using Hhs.IdentityService.Domain.FakeDomain.Entities;
using HsnSoft.Base.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hhs.IdentityService.EntityFrameworkCore;

public sealed class IdentityServiceDbContext : BaseDbContext<IdentityServiceDbContext>
{
    public DbSet<Fake> Fakes { get; set; }

    public IdentityServiceDbContext(DbContextOptions<IdentityServiceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureIdentityService();
    }
}