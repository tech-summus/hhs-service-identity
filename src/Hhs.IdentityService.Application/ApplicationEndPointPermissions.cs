using HsnSoft.Base.Reflection;

namespace Hhs.IdentityService.Application;

public static class ApplicationEndPointPermissions
{
    private const string GroupName = "IdentityService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(ApplicationEndPointPermissions));
    }

    public static class Fakes
    {
        private const string DomainName = GroupName + ".Fakes";

        public const string View = DomainName + ".View";
        public const string Detail = DomainName + ".Detail";
        public const string Update = DomainName + ".Update";
        public const string Delete = DomainName + ".Delete";

        public static class DetailFields
        {
            public const string Price = Detail + ".Price";
        }
    }
}