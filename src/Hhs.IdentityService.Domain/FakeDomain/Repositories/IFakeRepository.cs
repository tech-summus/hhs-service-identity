using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using HsnSoft.Base.Domain.Repositories;
using JetBrains.Annotations;

namespace Hhs.IdentityService.Domain.FakeDomain.Repositories;

public interface IFakeRepository : IReadOnlyGenericRepository<Fake, Guid>
{
    Task<Fake> CreateAsync(DateTime fakeDate, [NotNull] string fakeCode, FakeState fakeState, CancellationToken cancellationToken = default);

    Task<Fake> CreateAsync(Guid id, DateTime fakeDate, [NotNull] string fakeCode, FakeState fakeState, CancellationToken cancellationToken = default);

    Task<Fake> UpdateAsync(Guid id, DateTime fakeDate, [NotNull] string fakeCode, FakeState fakeState, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Fake>> GetPagedListWithFiltersAsync(
        DateTime? fakeDate = null,
        [CanBeNull] string fakeCode = null,
        FakeState? fakeState = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountWithFiltersAsync(
        DateTime? fakeDate = null,
        [CanBeNull] string fakeCode = null,
        FakeState? fakeState = null,
        CancellationToken cancellationToken = default
    );

    Task<List<Fake>> GetFilterListAsync(
        DateTime? fakeDate = null,
        [CanBeNull] string fakeCode = null,
        FakeState? fakeState = null,
        string sorting = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    Task<List<Fake>> GetSearchListAsync(
        string searchText = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    );
}