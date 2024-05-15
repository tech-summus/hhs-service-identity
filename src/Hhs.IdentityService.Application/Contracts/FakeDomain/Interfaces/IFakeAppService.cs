using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Filters;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Submits;
using HsnSoft.Base.Application.Dtos;
using HsnSoft.Base.EventBus;

namespace Hhs.IdentityService.Application.Contracts.FakeDomain.Interfaces;

public interface IFakeAppService: IEventApplicationService
{
    Task<FakeDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResultDto<FakeDto>> GetPagedListAsync(GetFakesPaged pagedInput, CancellationToken cancellationToken = default);
    Task<List<FakeDto>> GetFilterListAsync(GetFakesFilter filterInput, CancellationToken cancellationToken = default);
    Task<List<FakeSearchDto>> GetSearchListAsync(GetFakesSearch searchInput, CancellationToken cancellationToken = default);

    Task<FakeDto> CreateAsync(FakeCreateDto input);

    Task UpdateAsync(FakeUpdateDto input);

    Task DeleteAsync(Guid id);
}