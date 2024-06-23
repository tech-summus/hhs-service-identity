using System.Linq.Expressions;
using Hhs.IdentityService.Domain.AppUserDomain.Entities;
using JetBrains.Annotations;

namespace Hhs.IdentityService.Domain.AppUserDomain.Repositories;

public interface IAppUserRepository
{
    Task<List<AppUser>> GetPagedListWithFiltersAsync(
        Guid? tenantId, bool? includeDeletedRoles,
        [CanBeNull] string username = null,
        [CanBeNull] string email = null,
        bool? emailConfirmed = null,
        [CanBeNull] string phoneNumber = null,
        bool? phoneConfirmed = null,
        [CanBeNull] string name = null,
        [CanBeNull] string surname = null,
        List<Guid> roleIds = null,
        [CanBeNull] string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountWithFiltersAsync(
        Guid? tenantId, bool? includeDeletedRoles,
        [CanBeNull] string username = null,
        [CanBeNull] string email = null,
        bool? emailConfirmed = null,
        [CanBeNull] string phoneNumber = null,
        bool? phoneConfirmed = null,
        [CanBeNull] string name = null,
        [CanBeNull] string surname = null,
        List<Guid> roleIds = null,
        CancellationToken cancellationToken = default
    );

    Task<List<AppUser>> GetFilterListAsync(
        Guid? tenantId, bool? includeDeletedRoles,
        [CanBeNull] string username = null,
        [CanBeNull] string email = null,
        bool? emailConfirmed = null,
        [CanBeNull] string phoneNumber = null,
        bool? phoneConfirmed = null,
        [CanBeNull] string name = null,
        [CanBeNull] string surname = null,
        List<Guid> roleIds = null,
        [CanBeNull] string sorting = null,
        CancellationToken cancellationToken = default
    );

    Task<List<AppUser>> GetSearchListAsync(
        Guid? tenantId, bool? includeDeletedRoles,
        [CanBeNull] string searchText = null,
        [CanBeNull] string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    );

    //default functions
    [ItemCanBeNull]
    Task<AppUser> FindWithIdAsync(Guid? tenantId, Guid id);

    [ItemCanBeNull]
    Task<AppUser> FindAsync(Guid? tenantId, Expression<Func<AppUser, bool>> predicate);
}