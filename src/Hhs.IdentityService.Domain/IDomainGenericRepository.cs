using HsnSoft.Base.Domain.Entities;
using HsnSoft.Base.Domain.Repositories;

namespace Hhs.IdentityService.Domain;

public interface IDomainGenericRepository<TEntity> : IGenericRepository<TEntity, Guid>
    where TEntity : class, IEntity<Guid>
{
}