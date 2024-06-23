using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Hhs.IdentityService.Domain.AppUserDomain.Consts;
using Hhs.IdentityService.Domain.AppUserDomain.Entities;
using Hhs.IdentityService.Domain.AppUserDomain.Repositories;
using Hhs.IdentityService.EntityFrameworkCore.Context;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Hhs.IdentityService.EntityFrameworkCore.Repositories;

public class AppUserRepository : IAppUserRepository
{
    private readonly IdentityAppDbContext _context;

    public AppUserRepository(IdentityAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppUser>> GetPagedListWithFiltersAsync(Guid? tenantId, bool? includeDeletedUsers,
        string username = null,
        string email = null,
        bool? emailConfirmed = null,
        string phoneNumber = null,
        bool? phoneConfirmed = null,
        string name = null,
        string surname = null,
        List<Guid> roleIds = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    )
    {
        var queryUser = _context.Users.AsQueryable();
        if (roleIds is { Count: > 0 })
        {
            queryUser = from usr in _context.Users
                join ur in _context.UserRoles on usr.Id equals ur.UserId
                where roleIds.Contains(ur.RoleId)
                select usr;
        }

        var query = ApplyFilter(queryUser, tenantId, includeDeletedUsers, null,
            username, email, emailConfirmed, phoneNumber, phoneConfirmed, name, surname);

        return await query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? AppUserConsts.GetDefaultSorting(false) : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<long> GetCountWithFiltersAsync(Guid? tenantId, bool? includeDeletedUsers,
        string username = null,
        string email = null,
        bool? emailConfirmed = null,
        string phoneNumber = null,
        bool? phoneConfirmed = null,
        string name = null,
        string surname = null,
        List<Guid> roleIds = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryUser = _context.Users.AsQueryable();
        if (roleIds is { Count: > 0 })
        {
            queryUser = from usr in _context.Users
                join ur in _context.UserRoles on usr.Id equals ur.UserId
                where roleIds.Contains(ur.RoleId)
                select usr;
        }

        var query = ApplyFilter(queryUser, tenantId, includeDeletedUsers, null,
            username, email, emailConfirmed, phoneNumber, phoneConfirmed, name, surname);

        return await query.LongCountAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<AppUser>> GetFilterListAsync(Guid? tenantId, bool? includeDeletedUsers,
        string username = null,
        string email = null,
        bool? emailConfirmed = null,
        string phoneNumber = null,
        bool? phoneConfirmed = null,
        string name = null,
        string surname = null,
        List<Guid> roleIds = null,
        string sorting = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryUser = _context.Users.AsQueryable();
        if (roleIds is { Count: > 0 })
        {
            queryUser = from usr in _context.Users
                join ur in _context.UserRoles on usr.Id equals ur.UserId
                where roleIds.Contains(ur.RoleId)
                select usr;
        }

        var query = ApplyFilter(queryUser, tenantId, includeDeletedUsers, null,
            username, email, emailConfirmed, phoneNumber, phoneConfirmed, name, surname);

        return await query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? AppUserConsts.GetDefaultSorting(false) : sorting)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<AppUser>> GetSearchListAsync(Guid? tenantId, bool? includeDeletedUsers,
        string searchText = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyFilter(_context.Users.AsQueryable(), tenantId, includeDeletedUsers, searchText);

        return await query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? AppUserConsts.GetDefaultSorting(false) : sorting)
            .PageBy(0, maxResultCount)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<AppUser> FindWithIdAsync(Guid? tenantId, Guid id)
        => await FindAsync(tenantId, x => x.Id == id);

    public async Task<AppUser> FindAsync(Guid? tenantId, Expression<Func<AppUser, bool>> predicate)
        => await _context.Users.Where(x => x.IsDeleted == false)
            .WhereIf(tenantId.HasValue, e => e.TenantId == tenantId.Value)
            .FirstOrDefaultAsync(predicate);

    private IQueryable<AppUser> ApplyFilter(
        IQueryable<AppUser> query,
        Guid? tenantId,
        bool? includeDeletedUsers,
        [CanBeNull] string searchText = null,
        [CanBeNull] string username = null,
        [CanBeNull] string email = null,
        bool? emailConfirmed = null,
        [CanBeNull] string phoneNumber = null,
        bool? phoneConfirmed = null,
        [CanBeNull] string name = null,
        [CanBeNull] string surname = null)
    {
        searchText = searchText?.ToLower();
        username = username?.ToLower();
        email = email?.ToLower();
        phoneNumber = phoneNumber?.ToLower();
        name = name?.ToLower();
        surname = surname?.ToLower();

        if (!(includeDeletedUsers.HasValue && includeDeletedUsers.Value))
        {
            query = query.Where(x => x.IsDeleted == false);
        }

        return query
            .WhereIf(tenantId.HasValue, e => e.TenantId == tenantId.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(searchText), e => e.UserName.ToLower().Contains(searchText)
                                                                  || e.Email.ToLower().Contains(searchText)
                                                                  || e.Name.ToLower().Contains(searchText)
                                                                  || e.Surname.ToLower().Contains(searchText))
            .WhereIf(!string.IsNullOrWhiteSpace(username), e => e.UserName.ToLower().Contains(username))
            .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Email.ToLower().Contains(email))
            .WhereIf(emailConfirmed.HasValue, e => e.EmailConfirmed == emailConfirmed)
            .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.ToLower().StartsWith(phoneNumber))
            .WhereIf(phoneConfirmed.HasValue, e => e.PhoneNumberConfirmed == phoneConfirmed)
            .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.ToLower().Contains(name))
            .WhereIf(!string.IsNullOrWhiteSpace(surname), e => e.Surname.ToLower().Contains(surname));
    }
}