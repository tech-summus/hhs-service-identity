using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using HsnSoft.Base.Domain.Repositories;
using JetBrains.Annotations;

namespace Hhs.IdentityService.Domain.FakeDomain.Repositories;

public interface IFakeReadOnlyRepository : IReadOnlyRepository<Fake, Guid>
{
    Task<List<Fake>> GetPagedListWithFiltersAsync(
        DateTime? fakeDate = null,
        [CanBeNull] string fakeCode = null,
        FakeState? fakeState = null,
        [CanBeNull] string sorting = null,
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
        [CanBeNull] string sorting = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    Task<List<Fake>> GetSearchListAsync(
        [CanBeNull] string searchText = null,
        [CanBeNull] string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    );
}