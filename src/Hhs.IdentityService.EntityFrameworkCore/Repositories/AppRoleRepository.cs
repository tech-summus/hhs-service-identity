using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Hhs.IdentityService.Domain.AppRoleDomain.Consts;
using Hhs.IdentityService.Domain.AppRoleDomain.Entities;
using Hhs.IdentityService.Domain.AppRoleDomain.Repositories;
using Hhs.IdentityService.EntityFrameworkCore.Context;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Hhs.IdentityService.EntityFrameworkCore.Repositories;

public class AppRoleRepository : IAppRoleRepository
{
    private readonly IdentityAppDbContext _context;

    public AppRoleRepository(IdentityAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppRole>> GetPagedListWithFiltersAsync(Guid? tenantId, bool? includeDeletedRoles,
        string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyFilter(_context.Roles.AsQueryable(), tenantId, includeDeletedRoles, null,
            name, isDefault, isStatic, isPublic);

        return await query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? AppRoleConsts.GetDefaultSorting(false) : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<long> GetCountWithFiltersAsync(Guid? tenantId, bool? includeDeletedRoles,
        string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyFilter(_context.Roles.AsQueryable(), tenantId, includeDeletedRoles, null,
            name, isDefault, isStatic, isPublic);

        return await query.LongCountAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<AppRole>> GetFilterListAsync(Guid? tenantId, bool? includeDeletedRoles,
        [CanBeNull] string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null,
        string sorting = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyFilter(_context.Roles.AsQueryable(), tenantId, includeDeletedRoles, null,
            name, isDefault, isStatic, isPublic);

        return await query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? AppRoleConsts.GetDefaultSorting(false) : sorting)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<AppRole>> GetSearchListAsync(Guid? tenantId, bool? includeDeletedRoles,
        string searchText = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyFilter(_context.Roles.AsQueryable(), tenantId, includeDeletedRoles, searchText);

        return await query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? AppRoleConsts.GetDefaultSorting(false) : sorting)
            .PageBy(0, maxResultCount)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<AppRole> FindWithIdAsync(Guid? tenantId, Guid id)
        => await FindAsync(tenantId, x => x.Id == id);

    public async Task<AppRole> FindAsync(Guid? tenantId, Expression<Func<AppRole, bool>> predicate)
        => await _context.Roles.Where(x => x.IsDeleted == false)
            .WhereIf(tenantId.HasValue, e => e.TenantId == tenantId.Value)
            .FirstOrDefaultAsync(predicate);

    private IQueryable<AppRole> ApplyFilter(
        IQueryable<AppRole> query,
        Guid? tenantId,
        bool? includeDeletedRoles,
        [CanBeNull] string searchText = null,
        [CanBeNull] string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null)
    {
        searchText = searchText?.ToLower();
        name = name?.ToLower();

        if (!(includeDeletedRoles.HasValue && includeDeletedRoles.Value))
        {
            query = query.Where(x => x.IsDeleted == false);
        }

        return query
            .WhereIf(tenantId.HasValue, e => e.TenantId == tenantId.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(searchText), e => e.Name.ToLower().Contains(searchText))
            .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.ToLower().Contains(name))
            .WhereIf(isDefault.HasValue, e => e.IsDefault == isDefault)
            .WhereIf(isStatic.HasValue, e => e.IsStatic == isStatic)
            .WhereIf(isPublic.HasValue, e => e.IsPublic == isPublic);
    }
}