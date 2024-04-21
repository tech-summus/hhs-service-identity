using System.Net;
using Hhs.IdentityService.Application.Contracts.Events;
using Hhs.IdentityService.Application.Contracts.FakeDomain;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Filters;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Submits;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Interfaces;
using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using Hhs.IdentityService.Domain.FakeDomain.Repositories;
using Hhs.IdentityService.Domain.FakeDomain.Services;
using HsnSoft.Base;
using HsnSoft.Base.Application.Dtos;
using HsnSoft.Base.DependencyInjection;
using HsnSoft.Base.Domain.Entities.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hhs.IdentityService.Application.Services;

public sealed class FakeAppService : IdentityServiceAppService, IFakeAppService
{
    private readonly ILogger<FakeAppService> _logger;
    private readonly FakeSettings _settings;
    private readonly IFakeReadOnlyRepository _fakeRepository;
    private readonly FakeManager _fakeManager;

    public FakeAppService(IBaseLazyServiceProvider provider,
        ILogger<FakeAppService> logger,
        IOptions<FakeSettings> settings,
        IFakeReadOnlyRepository fakeRepository,
        FakeManager fakeManager
    ) : base(provider)
    {
        _logger = logger;
        _settings = settings.Value;
        _fakeManager = fakeManager;
        _fakeRepository = fakeRepository;

        _logger.LogTrace("FakeSettings: {@FakeSettings}", _settings);
    }

    public async Task<FakeDto> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var item = await _fakeRepository.FindAsync(id);
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

        var totalCount = await _fakeRepository.GetCountWithFiltersAsync(null, pagedInput.FakeCode, pagedInput.FakeState);

        var items = await _fakeRepository.GetPagedListWithFiltersAsync(null, pagedInput.FakeCode, pagedInput.FakeState,
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

        var items = await _fakeRepository.GetFilterListAsync(null, filterInput.FakeCode, filterInput.FakeState,
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

        var items = await _fakeRepository.GetSearchListAsync(searchInput.SearchText,
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

        var placedFake = await _fakeManager.CreateAsync(
            fakeDate: input.FakeDate ?? DateTime.Now,
            fakeCode: input.FakeCode ?? string.Empty,
            fakeState: input.FakeState ?? FakeState.WaitForApprove
        );

        //TODO: OPERATION WILL BE INTEGRATED
        IIntegrationEventMessage operationResultIntegrationEvent;
        // The operation can be successful or it can fail
        if (_settings.OperationSucceeded)
        {
            Thread.Sleep(5000);
            await UpdateAsync(new FakeUpdateDto { Id = placedFake.Id, FakeDate = placedFake.FakeDate, FakeState = FakeState.ResultSuccess });
            operationResultIntegrationEvent = new OperationSucceededEto(placedFake.Id);
        }
        else
        {
            Thread.Sleep(5000);
            await UpdateAsync(new FakeUpdateDto { Id = placedFake.Id, FakeDate = placedFake.FakeDate, FakeState = FakeState.ResultFail });
            operationResultIntegrationEvent = new OperationFailedEto(placedFake.Id);
        }

        await PublishEventAsync(operationResultIntegrationEvent);

        return Mapper.Map<Fake, FakeDto>(placedFake);
    }

    public async Task UpdateAsync(FakeUpdateDto input)
    {
        if (input == null || input.Id == Guid.Empty)
        {
            throw new BaseHttpException((int)HttpStatusCode.BadRequest);
        }

        var fake = await _fakeManager.UpdateAsync(
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

        await _fakeManager.DeleteAsync(id);

        //INTEGRATION EVENT TRIGGER
        //
        //
    }
}