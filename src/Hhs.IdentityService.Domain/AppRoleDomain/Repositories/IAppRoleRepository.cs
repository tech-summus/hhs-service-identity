using System.Linq.Expressions;
using Hhs.IdentityService.Domain.AppRoleDomain.Entities;
using JetBrains.Annotations;

namespace Hhs.IdentityService.Domain.AppRoleDomain.Repositories;

public interface IAppRoleRepository
{
    Task<List<AppRole>> GetPagedListWithFiltersAsync(
        Guid? tenantId, bool? includeDeletedRoles,
        [CanBeNull] string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null,
        [CanBeNull] string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountWithFiltersAsync(
        Guid? tenantId, bool? includeDeletedRoles,
        [CanBeNull] string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null,
        CancellationToken cancellationToken = default
    );

    Task<List<AppRole>> GetFilterListAsync(
        Guid? tenantId, bool? includeDeletedRoles,
        [CanBeNull] string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null,
        [CanBeNull] string sorting = null,
        CancellationToken cancellationToken = default
    );

    Task<List<AppRole>> GetSearchListAsync(
        Guid? tenantId, bool? includeDeletedRoles,
        [CanBeNull] string searchText = null,
        [CanBeNull] string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    );


    //default functions
    [ItemCanBeNull]
    Task<AppRole> FindWithIdAsync(Guid? tenantId, Guid id);

    [ItemCanBeNull]
    Task<AppRole> FindAsync(Guid? tenantId, Expression<Func<AppRole, bool>> predicate);
}