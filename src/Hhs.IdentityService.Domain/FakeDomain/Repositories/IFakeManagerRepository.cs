using Hhs.IdentityService.Domain.FakeDomain.Entities;
using HsnSoft.Base.Domain.Repositories;

namespace Hhs.IdentityService.Domain.FakeDomain.Repositories;

public interface IFakeManagerRepository : IRepository<Fake, Guid>, IFakeReadOnlyRepository
{
}