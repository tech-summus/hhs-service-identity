using Hhs.IdentityService.Application.Contracts.FakeDomain;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Filters;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Submits;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Interfaces;
using Hhs.IdentityService.Controllers.Base;
using HsnSoft.Base.Application.Dtos;
using HsnSoft.Base.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace Hhs.IdentityService.Controllers;

[ControllerName("Fake")]
[Route("api/v1/identity-service/commercial/fakes")]
public sealed class FakeController : IdentityServiceController, IFakeAppService
{
    private readonly IFakeAppService _fakeAppService;

    public FakeController(IBaseLazyServiceProvider provider, IFakeAppService fakeAppService) : base(provider)
    {
        _fakeAppService = fakeAppService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<FakeDto> GetAsync(Guid id) => await _fakeAppService.GetAsync(id);

    [HttpPost("paged-list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<PagedResultDto<FakeDto>> GetPagedListAsync([FromBody] GetFakesPaged pagedInput) => await _fakeAppService.GetPagedListAsync(pagedInput);

    [HttpPost("filter-list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<List<FakeDto>> GetFilterListAsync([FromBody] GetFakesFilter filterInput) => await _fakeAppService.GetFilterListAsync(filterInput);

    [HttpPost("search-list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<List<FakeSearchDto>> GetSearchListAsync([FromBody] GetFakesSearch searchInput) => await _fakeAppService.GetSearchListAsync(searchInput);

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<FakeDto> CreateAsync([FromBody] FakeCreateDto input) => await _fakeAppService.CreateAsync(input);

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task UpdateAsync([FromBody] FakeUpdateDto input) => await _fakeAppService.UpdateAsync(input);

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task DeleteAsync(Guid id) => await _fakeAppService.DeleteAsync(id);
}