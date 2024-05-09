using AutoMapper;
using Hhs.IdentityService.Application.Contracts.FakeDomain;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos;
using Hhs.IdentityService.Domain.FakeDomain.Entities;

namespace Hhs.IdentityService.Application;

public class ApplicationAutoMapperProfile : Profile
{
    public ApplicationAutoMapperProfile()
    {
        CreateMap<Fake, FakeDto>()
            .ForMember(dest => dest.FakeCode, opt =>
                opt.MapFrom(source => source.FakeCode));

        CreateMap<FakeDto, Fake>();
    }
}