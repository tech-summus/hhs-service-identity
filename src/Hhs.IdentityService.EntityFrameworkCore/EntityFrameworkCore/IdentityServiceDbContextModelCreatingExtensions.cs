using Hhs.IdentityService.Domain;
using Hhs.IdentityService.Domain.FakeDomain;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using HsnSoft.Base;
using Microsoft.EntityFrameworkCore;

namespace Hhs.IdentityService.EntityFrameworkCore;

public static class IdentityServiceDbContextModelCreatingExtensions
{
    public static void ConfigureIdentityService(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Fake>(b =>
        {
            b.ToTable(IdentityServiceDbProperties.DbTablePrefix + FakeConsts.TableName, IdentityServiceDbProperties.DbSchema);
            b.HasKey(ci => ci.Id);

            b.Property(x => x.FakeDate).HasColumnName(nameof(Fake.FakeDate)).IsRequired();
            b.Property(x => x.FakeCode).HasColumnName(nameof(Fake.FakeCode)).IsRequired().HasMaxLength(FakeConsts.FakeCodeMaxLength);
            b.Property(x => x.FakeState).HasColumnName(nameof(Fake.FakeState)).IsRequired();
        });
    }
}