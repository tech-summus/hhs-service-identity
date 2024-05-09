using System.Net;
using Hhs.IdentityService.Application.Contracts.FakeDomain;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Filters;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Submits;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Interfaces;
using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using Hhs.IdentityService.Domain.FakeDomain.Services;
using HsnSoft.Base;
using HsnSoft.Base.Application.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hhs.IdentityService.Application.Services;

public sealed class FakeAppService : ApplicationServiceBase, IFakeAppService
{
    private readonly ILogger _logger;
    private readonly FakeDomainService _fakeDomainService;

    public FakeAppService(IServiceProvider provider,
        IOptions<FakeSettings> settings,
        FakeDomainService fakeDomainService
    ) : base(provider)
    {
        _logger = LoggerFactory.CreateLogger<FakeAppService>();
        _fakeDomainService = fakeDomainService;

        _logger.LogTrace("FakeSettings: {@FakeSettings}", settings.Value);
    }

    public async Task<FakeDto> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var item = await _fakeDomainService.FindAsync(id);
        if (item == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.NotFound);
        }

        return Mapper.Map<Fake, FakeDto>(item);
    }

    public async Task<PagedResultDto<FakeDto>> GetPagedListAsync(GetFakesPaged pagedInput)
    {
        if (pagedInput == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var totalCount = await _fakeDomainService.GetCountWithFiltersAsync(null, pagedInput.FakeCode, pagedInput.FakeState);

        var items = await _fakeDomainService.GetPagedListWithFiltersAsync(null, pagedInput.FakeCode, pagedInput.FakeState,
            pagedInput.Sorting, pagedInput.MaxResultCount, pagedInput.SkipCount, true);

        if (items == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.RequestTimeout);
        }

        return new PagedResultDto<FakeDto> { TotalCount = totalCount, Items = Mapper.Map<List<Fake>, List<FakeDto>>(items) };
    }

    public async Task<List<FakeDto>> GetFilterListAsync(GetFakesFilter filterInput)
    {
        if (filterInput == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var items = await _fakeDomainService.GetFilterListAsync(null, filterInput.FakeCode, filterInput.FakeState,
            filterInput.Sorting, true);

        if (items == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.RequestTimeout);
        }

        return Mapper.Map<List<Fake>, List<FakeDto>>(items);
    }

    public async Task<List<FakeSearchDto>> GetSearchListAsync(GetFakesSearch searchInput)
    {
        if (searchInput == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var items = await _fakeDomainService.GetSearchListAsync(searchInput.SearchText,
            searchInput.Sorting, searchInput.MaxResultCount);

        if (items == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.RequestTimeout);
        }

        return Mapper.Map<List<Fake>, List<FakeSearchDto>>(items);
    }

    public async Task<FakeDto> CreateAsync(FakeCreateDto input)
    {
        if (input == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var placedFake = await _fakeDomainService.CreateAsync(
            fakeDate: input.FakeDate ?? DateTime.Now,
            fakeCode: input.FakeCode ?? string.Empty,
            fakeState: input.FakeState ?? FakeState.WaitForApprove
        );

        return Mapper.Map<Fake, FakeDto>(placedFake);
    }

    public async Task UpdateAsync(FakeUpdateDto input)
    {
        if (input == null || input.Id == Guid.Empty)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var fake = await _fakeDomainService.UpdateAsync(
            id: input.Id,
            fakeDate: input.FakeDate ?? DateTime.Now,
            fakeCode: input.FakeCode ?? string.Empty,
            fakeState: input.FakeState ?? FakeState.WaitForApprove
        );

        //INTEGRATION EVENT TRIGGER
        //
        //
    }

    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        await _fakeDomainService.DeleteAsync(id);

        //INTEGRATION EVENT TRIGGER
        //
        //
    }
}