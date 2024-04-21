using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Filters;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Submits;
using HsnSoft.Base.Application.Dtos;

namespace Hhs.IdentityService.Application.Contracts.FakeDomain.Interfaces;

public interface IFakeAppService
{
    Task<FakeDto> GetAsync(Guid id);

    Task<PagedResultDto<FakeDto>> GetPagedListAsync(GetFakesPaged pagedInput);
    Task<List<FakeDto>> GetFilterListAsync(GetFakesFilter filterInput);
    Task<List<FakeSearchDto>> GetSearchListAsync(GetFakesSearch searchInput);

    Task<FakeDto> CreateAsync(FakeCreateDto input);

    Task UpdateAsync(FakeUpdateDto input);

    Task DeleteAsync(Guid id);
}