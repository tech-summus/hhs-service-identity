using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain.Consts;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using Hhs.IdentityService.Domain.FakeDomain.Exceptions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Hhs.IdentityService.Domain.FakeDomain.Services;

public sealed class FakeDomainService : DomainServiceBase
{
    private readonly ILogger _logger;
    private readonly IDomainGenericRepository<Fake> _fakeRepository;

    public FakeDomainService(IServiceProvider provider, IDomainGenericRepository<Fake> fakeRepository) : base(provider)
    {
        _logger = LoggerFactory.CreateLogger<FakeDomainService>();
        _fakeRepository = fakeRepository;
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

        return await _fakeRepository.InsertAsync(draftFake, cancellationToken);
    }

    public async Task<Fake> UpdateAsync(Guid id,
        DateTime fakeDate,
        [NotNull] string fakeCode,
        FakeState fakeState,
        CancellationToken cancellationToken = default)
    {
        var oldFake = await _fakeRepository.FindAsync(id, false, cancellationToken);
        if (oldFake == null)
        {
            _logger.LogDebug("Fake entity not found [{FakeId}]", id);
            throw new FakeNotFoundException(id.ToString());
        }

        oldFake.SetFakeDate(fakeDate);
        oldFake.SetFakeCode(fakeCode);
        oldFake.FakeState = fakeState;

        //Domain Rules
        // Rule01
        // Rule02

        return await _fakeRepository.UpdateAsync(oldFake, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fake = await _fakeRepository.FindAsync(id, false, cancellationToken);
        if (fake == null)
        {
            _logger.LogDebug("Fake entity not found [{FakeId}]", id);
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
        await _fakeRepository.UpdateAsync(fake, cancellationToken);
    }

    public async Task<Fake> FindAsync(Guid fakeId, CancellationToken cancellationToken = default)
    {
        return await _fakeRepository.FindAsync(fakeId, cancellationToken: cancellationToken);
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
        throw new NotImplementedException();
        //
        // var queryable = includeDetails
        //     ? await WithDetailsAsync(DefaultPropertySelector?.ToArray())
        //     : await GetQueryableAsync();
        //
        // var query = ApplyFilter(queryable, null,
        //     fakeDate, fakeCode, fakeState);
        //
        // return await query
        //     .OrderBy(string.IsNullOrWhiteSpace(sorting) ? FakeConsts.GetDefaultSorting(false) : sorting)
        //     .PageBy(skipCount, maxResultCount)
        //     .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountWithFiltersAsync(
        DateTime? fakeDate = null,
        [CanBeNull] string fakeCode = null,
        FakeState? fakeState = null,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
        // var query = ApplyFilter(await GetQueryableAsync(), null,
        //     fakeDate, fakeCode, fakeState);
        //
        // return await query.LongCountAsync(GetCancellationToken(cancellationToken));
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
        throw new NotImplementedException();
        // var queryable = includeDetails
        //     ? await WithDetailsAsync(DefaultPropertySelector?.ToArray())
        //     : await GetQueryableAsync();
        //
        // var query = ApplyFilter(queryable, null,
        //     fakeDate, fakeCode, fakeState);
        //
        // return await query
        //     .OrderBy(string.IsNullOrWhiteSpace(sorting) ? FakeConsts.GetDefaultSorting(false) : sorting)
        //     .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Fake>> GetSearchListAsync(
        string searchText = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
        // var query = ApplyFilter(await GetQueryableAsync(), searchText);
        //
        // return await query
        //     .OrderBy(string.IsNullOrWhiteSpace(sorting) ? FakeConsts.GetDefaultSorting(false) : sorting)
        //     .PageBy(0, maxResultCount)
        //     .ToListAsync(GetCancellationToken(cancellationToken));
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