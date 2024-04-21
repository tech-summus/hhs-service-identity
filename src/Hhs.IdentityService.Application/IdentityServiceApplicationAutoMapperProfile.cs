using AutoMapper;
using Hhs.IdentityService.Application.Contracts.FakeDomain;
using Hhs.IdentityService.Domain.FakeDomain.Entities;

namespace Hhs.IdentityService.Application;

public class IdentityServiceApplicationAutoMapperProfile : Profile
{
    public IdentityServiceApplicationAutoMapperProfile()
    {
        CreateMap<Fake, FakeDto>()
            .ForMember(dest => dest.FakeCode, opt =>
                opt.MapFrom(source => source.FakeCode));

        CreateMap<FakeDto, Fake>();
    }
}