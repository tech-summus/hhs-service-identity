using System.Linq.Dynamic.Core;
using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain.Consts;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using Hhs.IdentityService.Domain.FakeDomain.Exceptions;
using Hhs.IdentityService.Domain.FakeDomain.Repositories;
using Hhs.IdentityService.EntityFrameworkCore.Context;
using HsnSoft.Base.Domain.Repositories.EntityFrameworkCore;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Hhs.IdentityService.EntityFrameworkCore.Repositories;

public sealed class EfCoreFakeRepository : EfCoreGenericRepository<IdentityServiceDbContext, Fake, Guid>, IFakeRepository
{
    public EfCoreFakeRepository(IServiceProvider provider, IdentityServiceDbContext dbContext) : base(provider, dbContext)
    {
    }

    public async Task<Fake> CreateAsync(
        DateTime fakeDate,
        [NotNull] string fakeCode,
        FakeState fakeState,
        CancellationToken cancellationToken = default)
        => await CreateAsync(Guid.NewGuid(),
            fakeDate,
            fakeCode,
            fakeState,
            cancellationToken);

    public async Task<Fake> CreateAsync(
        Guid id,
        DateTime fakeDate,
        [NotNull] string fakeCode,
        FakeState fakeState,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) id = Guid.NewGuid();

        // Create draft Fake
        var draftFake = new Fake(
            id: id,
            fakeDate: fakeDate,
            fakeCode: fakeCode,
            fakeState: fakeState
        );

        //Domain Rules
        // Rule01
        // Rule02

        return await InsertAsync(draftFake, cancellationToken);
    }

    public async Task<Fake> UpdateAsync(Guid id,
        DateTime fakeDate,
        [NotNull] string fakeCode,
        FakeState fakeState,
        CancellationToken cancellationToken = default)
    {
        var oldFake = await FindAsync(id, false, cancellationToken);
        if (oldFake == null)
        {
            throw new FakeNotFoundException(id.ToString());
        }

        oldFake.SetFakeDate(fakeDate);
        oldFake.SetFakeCode(fakeCode);
        oldFake.FakeState = fakeState;

        //Domain Rules
        // Rule01
        // Rule02

        return await UpdateAsync(oldFake, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fake = await FindAsync(id, false, cancellationToken);
        if (fake == null)
        {
            throw new FakeNotFoundException(id.ToString());
        }

        //Domain Rule -> Fake Dependency Control for Delete
        // Rule01

        var guidGenerated = Guid.NewGuid().ToString("N").ToUpper();
        var uniqueField = guidGenerated + "_" + fake.FakeCode;
        if (uniqueField.Length > FakeConsts.FakeCodeMaxLength)
        {
            uniqueField = uniqueField[..FakeConsts.FakeCodeMaxLength];
        }

        fake.SetFakeCode(uniqueField);
        fake.IsDeleted = true;
        await UpdateAsync(fake, cancellationToken);
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
            ? WithDetails(DefaultPropertySelector?.ToArray())
            : GetQueryable();

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
        var query = ApplyFilter(GetQueryable(), null,
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
            ? WithDetails(DefaultPropertySelector?.ToArray())
            : GetQueryable();

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
        var query = ApplyFilter(GetQueryable(), searchText);

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