using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain.Entities;
using Hhs.IdentityService.Domain.FakeDomain.Exceptions;
using Hhs.IdentityService.Domain.FakeDomain.Repositories;
using HsnSoft.Base.DependencyInjection;
using JetBrains.Annotations;

namespace Hhs.IdentityService.Domain.FakeDomain.Services;

public sealed class FakeManager : IdentityServiceManager
{
    private readonly IFakeManagerRepository _fakeManagerRepository;

    public FakeManager(IBaseLazyServiceProvider provider,
        IFakeManagerRepository fakeManagerRepository) : base(provider)
    {
        _fakeManagerRepository = fakeManagerRepository;
    }

    public async Task<Fake> CreateAsync(
        DateTime fakeDate,
        [NotNull] string fakeCode,
        FakeState fakeState)
        => await CreateAsync(GuidGenerator.Create(),
            fakeDate,
            fakeCode,
            fakeState);

    public async Task<Fake> CreateAsync(
        Guid id,
        DateTime fakeDate,
        [NotNull] string fakeCode,
        FakeState fakeState)
    {
        if (id == Guid.Empty) id = GuidGenerator.Create();

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

        return await _fakeManagerRepository.InsertAsync(draftFake, true);
    }

    public async Task<Fake> UpdateAsync(Guid id,
        DateTime fakeDate,
        [NotNull] string fakeCode,
        FakeState fakeState)
    {
        var oldFake = await _fakeManagerRepository.FindAsync(id, false);
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

        return await _fakeManagerRepository.UpdateAsync(oldFake, true);
    }

    public async Task DeleteAsync(Guid id, bool isHardDelete = false)
    {
        var fake = await _fakeManagerRepository.FindAsync(id, false);
        if (fake == null)
        {
            throw new FakeNotFoundException(id.ToString());
        }

        //Domain Rule -> Fake Dependency Control for Delete
        // Rule01

        var guidGenerated = GuidGenerator.Create().ToString("N").ToUpper();
        var uniqueField = guidGenerated + "_" + fake.FakeCode;
        if (uniqueField.Length > FakeConsts.FakeCodeMaxLength)
        {
            uniqueField = uniqueField[..FakeConsts.FakeCodeMaxLength];
        }

        fake.SetFakeCode(uniqueField);
        fake.IsDeleted = true;
        await _fakeManagerRepository.UpdateAsync(fake, true);
    }
}