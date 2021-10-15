using FreeSql;
using IdentityServer4.FreeSql.Storage.DbContexts;
using IdentityServer4.FreeSql.Storage.Interfaces;
using IdentityServer4.FreeSql.Storage.Options;
using IdentityServer4.FreeSql.Storage.TokenCleanup;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityServer4.FreeSql.Storage
{
    /// <summary>
    /// Extension methods to add FreeSql database support to IdentityServer.
    /// </summary>
    public static class IdentityServerServiceCollectionExtensions
    {
        /// <summary>
        /// Add Configuration DbContext to the DI system.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddConfigurationDbContext(this IServiceCollection services,
            Action<ConfigurationStoreOptions> storeOptionsAction = null)
        {
            return services.AddConfigurationDbContext<ConfigurationDbContext>(storeOptionsAction);
        }

        /// <summary>
        /// Add Configuration DbContext to the DI system.
        /// </summary>
        /// <typeparam name="TContext">The IConfigurationDbContext to use.</typeparam>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddConfigurationDbContext<TContext>(this IServiceCollection services,
        Action<ConfigurationStoreOptions> storeOptionsAction = null)
        where TContext : DbContext, IConfigurationDbContext
        {
            var options = new ConfigurationStoreOptions();
            services.AddSingleton(options);
            storeOptionsAction?.Invoke(options);

            if (options.ResolveDbContextOptions != null)
            {
                //services.AddDbContext<TContext>(options.ResolveDbContextOptions);                
                services.AddFreeDbContext<TContext>(options: options.ConfigureDbContext);

            }
            else
            {
                services.AddFreeDbContext<TContext>(dbCtxBuilder =>
                {
                    options.ConfigureDbContext?.Invoke(dbCtxBuilder);
                });
            }
            services.AddScoped<IConfigurationDbContext, TContext>();

            return services;
        }

        /// <summary>
        /// Adds operational DbContext to the DI system.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalDbContext(this IServiceCollection services,
            Action<OperationalStoreOptions> storeOptionsAction = null)
        {
            return services.AddOperationalDbContext<PersistedGrantDbContext>(storeOptionsAction);
        }

        /// <summary>
        /// Adds operational DbContext to the DI system.
        /// </summary>
        /// <typeparam name="TContext">The IPersistedGrantDbContext to use.</typeparam>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalDbContext<TContext>(this IServiceCollection services,
            Action<OperationalStoreOptions> storeOptionsAction = null)
            where TContext : DbContext, IPersistedGrantDbContext
        {
            var storeOptions = new OperationalStoreOptions();
            services.AddSingleton(storeOptions);
            storeOptionsAction?.Invoke(storeOptions);

            if (storeOptions.ResolveDbContextOptions != null)
            {
                //services.AddDbContext<TContext>(storeOptions.ResolveDbContextOptions);
                services.AddFreeDbContext<TContext>(storeOptions.ConfigureDbContext);
            }
            else
            {
                services.AddFreeDbContext<TContext>(dbCtxBuilder =>
                {
                    storeOptions.ConfigureDbContext?.Invoke(dbCtxBuilder);
                });
            }

            services.AddScoped<IPersistedGrantDbContext, TContext>();
            services.AddTransient<TokenCleanupService>();

            return services;
        }

        /// <summary>
        /// Adds an implementation of the IOperationalStoreNotification to the DI system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalStoreNotification<T>(this IServiceCollection services)
           where T : class, IOperationalStoreNotification
        {
            services.AddTransient<IOperationalStoreNotification, T>();
            return services;
        }
    }
}
