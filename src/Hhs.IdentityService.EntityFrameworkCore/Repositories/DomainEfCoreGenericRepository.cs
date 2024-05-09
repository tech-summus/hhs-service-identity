using Hhs.IdentityService.Domain;
using Hhs.IdentityService.EntityFrameworkCore.Context;
using HsnSoft.Base.Domain.Entities;
using HsnSoft.Base.Domain.Repositories.EntityFrameworkCore;

namespace Hhs.IdentityService.EntityFrameworkCore.Repositories;

public sealed class DomainEfCoreGenericRepository<TEntity> : EfCoreGenericRepository<IdentityServiceDbContext, TEntity, Guid>, IDomainGenericRepository<TEntity>
    where TEntity : class, IEntity<Guid>
{
    public DomainEfCoreGenericRepository(IServiceProvider provider, IdentityServiceDbContext dbContext) : base(provider, dbContext)
    {
    }
}