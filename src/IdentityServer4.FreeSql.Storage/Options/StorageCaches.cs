
namespace IdentityServer4.FreeSql.Storage.Options
{
    public class StorageCaches
    {
        public const string CLIENT_ALLRESOURCES = "ONCEMI_IDENTITY_CLIENT_ALLRESOURCES";

        public const string CLIENT = "ONCEMI_IDENTITY_CLIENT_{0}";

        public static string GetClientCacheKey(string clientId)
        {
            return string.Format(CLIENT, clientId);
        }
    }
}
