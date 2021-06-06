using IdentityServer4.FreeSql.Storage.DbContexts;
using IdentityServer4.FreeSql.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace OnceMi.IdentityServer4.Extensions
{
    public static class DbContextSqlLoggerExtension
    {
        #region SqlLoger

        private static ILogger<IFreeSql> _logger;

        public static void UseSqlLogger(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<IFreeSql>>();
                IFreeSql<ConfigurationDbContext> configurationDbContext = scope.ServiceProvider.GetService<IFreeSql<ConfigurationDbContext>>();
                if (configurationDbContext != null)
                {
                    BindLog(_logger, configurationDbContext, nameof(ConfigurationDbContext));
                }
                IFreeSql<PersistedGrantDbContext> persistedGrantDbContext = scope.ServiceProvider.GetService<IFreeSql<PersistedGrantDbContext>>();
                if (persistedGrantDbContext != null)
                {
                    BindLog(_logger, persistedGrantDbContext, nameof(PersistedGrantDbContext));
                }
                IFreeSql<UserDbContext> userDbContext = scope.ServiceProvider.GetService<IFreeSql<UserDbContext>>();
                if (userDbContext != null)
                {
                    BindLog(_logger, userDbContext, nameof(UserDbContext));
                }
            }
        }

        private static void BindLog(ILogger logger, IFreeSql fsql, string name)
        {
            fsql.Aop.CurdAfter += (s, e) =>
            {
                logger.LogDebug($"{nameof(UserDbContext)}({Thread.CurrentThread.ManagedThreadId}):\n  Namespace: {e.EntityType.FullName} \nElapsedTime: {e.ElapsedMilliseconds}ms \n        SQL: {e.Sql}");
            };
        }

        #endregion
    }
}
