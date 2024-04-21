using System.Linq.Dynamic.Core;
using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using Hhs.IdentityService.Domain.FakeDomain.Repositories;
using Hhs.IdentityService.EntityFrameworkCore;
using HsnSoft.Base.Domain.Repositories.EntityFrameworkCore;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Hhs.IdentityService.FakeDomain;

public sealed class EfCoreFakeRepository : EfCoreRepository<IdentityServiceDbContext, Fake, Guid>, IFakeManagerRepository
{
    public EfCoreFakeRepository(IServiceProvider serviceProvider, IdentityServiceDbContext dbContext)
        : base(serviceProvider, dbContext)
    {
        GetDbContext().ServiceProvider = serviceProvider;
        DefaultPropertySelector = null;
    }

    public async Task<List<Fake>> GetPagedListWithFiltersAsync(
        DateTime? fakeDate = null,
        [CanBeNull] string fakeCode = null,
        FakeState? fakeState = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = includeDetails
            ? await WithDetailsAsync(DefaultPropertySelector?.ToArray())
            : await GetQueryableAsync();

        var query = ApplyFilter(queryable, null,
            fakeDate, fakeCode, fakeState);

        return await query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? FakeConsts.GetDefaultSorting(false) : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountWithFiltersAsync(
        DateTime? fakeDate = null,
        [CanBeNull] string fakeCode = null,
        FakeState? fakeState = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyFilter(await GetQueryableAsync(), null,
            fakeDate, fakeCode, fakeState);

        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Fake>> GetFilterListAsync(
        DateTime? fakeDate = null,
        [CanBeNull] string fakeCode = null,
        FakeState? fakeState = null,
        string sorting = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = includeDetails
            ? await WithDetailsAsync(DefaultPropertySelector?.ToArray())
            : await GetQueryableAsync();

        var query = ApplyFilter(queryable, null,
            fakeDate, fakeCode, fakeState);

        return await query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? FakeConsts.GetDefaultSorting(false) : sorting)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Fake>> GetSearchListAsync(
        string searchText = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyFilter(await GetQueryableAsync(), searchText);

        return await query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? FakeConsts.GetDefaultSorting(false) : sorting)
            .PageBy(0, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    private IQueryable<Fake> ApplyFilter(
        IQueryable<Fake> query,
        [CanBeNull] string searchText = null,
        DateTime? fakeDate = null,
        [CanBeNull] string fakeCode = null,
        FakeState? fakeState = null
    )
    {
        searchText = searchText?.ToLower();
        fakeCode = fakeCode?.ToLower();

        if (fakeDate.HasValue)
        {
            DateTime fakeStartTime = fakeDate.Value.Date;
            DateTime fakeEndTime = fakeStartTime.AddDays(1);
            query = query.Where(x => x.FakeDate >= fakeStartTime && x.FakeDate < fakeEndTime);
        }

        return query
            .WhereIf(!string.IsNullOrWhiteSpace(searchText), e => e.FakeCode.ToLower().Contains(searchText))
            .WhereIf(!string.IsNullOrWhiteSpace(fakeCode), e => e.FakeCode.ToLower().Contains(fakeCode))
            .WhereIf(fakeState.HasValue, e => e.FakeState == fakeState.Value);
    }
}