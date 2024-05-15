using System.Net;
using Hhs.IdentityService.Application.Contracts.FakeDomain;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Filters;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Submits;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Interfaces;
using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using Hhs.IdentityService.Domain.FakeDomain.Repositories;
using HsnSoft.Base;
using HsnSoft.Base.Application.Dtos;
using HsnSoft.Base.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hhs.IdentityService.Application.Services;

public sealed class FakeAppService : ApplicationServiceBase, IFakeAppService
{
    private readonly IBaseLogger _logger;
    private readonly IFakeRepository _fakeRepository;

    public FakeAppService(IServiceProvider provider,
        IOptions<FakeSettings> settings,
        IFakeRepository fakeRepository
    ) : base(provider)
    {
        _logger = provider.GetRequiredService<IBaseLogger>();
        _fakeRepository = fakeRepository;

        _logger.LogDebug("FakeSettings: {@FakeSettings}", settings.Value);
    }

    public async Task<FakeDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var item = await _fakeRepository.FindAsync(id, cancellationToken: cancellationToken);
        if (item == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.NotFound);
        }

        return Mapper.Map<Fake, FakeDto>(item);
    }

    public async Task<PagedResultDto<FakeDto>> GetPagedListAsync(GetFakesPaged pagedInput, CancellationToken cancellationToken = default)
    {
        if (pagedInput == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var totalCount = await _fakeRepository.GetCountWithFiltersAsync(null, pagedInput.FakeCode, pagedInput.FakeState, cancellationToken);

        var items = await _fakeRepository.GetPagedListWithFiltersAsync(null, pagedInput.FakeCode, pagedInput.FakeState,
            pagedInput.Sorting, pagedInput.MaxResultCount, pagedInput.SkipCount, true, cancellationToken);

        if (items == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.RequestTimeout);
        }

        return new PagedResultDto<FakeDto> { TotalCount = totalCount, Items = Mapper.Map<List<Fake>, List<FakeDto>>(items) };
    }

    public async Task<List<FakeDto>> GetFilterListAsync(GetFakesFilter filterInput, CancellationToken cancellationToken = default)
    {
        if (filterInput == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var items = await _fakeRepository.GetFilterListAsync(null, filterInput.FakeCode, filterInput.FakeState,
            filterInput.Sorting, true, cancellationToken);

        if (items == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.RequestTimeout);
        }

        return Mapper.Map<List<Fake>, List<FakeDto>>(items);
    }

    public async Task<List<FakeSearchDto>> GetSearchListAsync(GetFakesSearch searchInput, CancellationToken cancellationToken = default)
    {
        if (searchInput == null)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var items = await _fakeRepository.GetSearchListAsync(searchInput.SearchText,
            searchInput.Sorting, searchInput.MaxResultCount, cancellationToken);

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

        var placedFake = await _fakeRepository.CreateAsync(
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

        var fake = await _fakeRepository.UpdateAsync(
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

        await _fakeRepository.RemoveAsync(id);

        //INTEGRATION EVENT TRIGGER
        //
        //
    }
}