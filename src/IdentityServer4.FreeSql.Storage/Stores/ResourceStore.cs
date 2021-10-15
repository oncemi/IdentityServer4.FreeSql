using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.FreeSql.Storage.Interfaces;
using IdentityServer4.FreeSql.Storage.Mappers;
using IdentityServer4.Stores;
using FreeSql;
using Microsoft.Extensions.Logging;
using IdentityServer4.FreeSql.Storage.Entities;
using Microsoft.Extensions.Caching.Memory;
using IdentityServer4.FreeSql.Storage.Options;

namespace IdentityServer4.FreeSql.Storage.Stores
{
    /// <summary>
    /// Implementation of IResourceStore thats uses FreeSql.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected readonly IConfigurationDbContext Context;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger<ResourceStore> Logger;

        /// <summary>
        /// 缓存
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public ResourceStore(IConfigurationDbContext context
            , IMemoryCache cache
            , ILogger<ResourceStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Logger = logger;
        }

        /// <summary>
        /// Finds the API resources by name.
        /// </summary>
        /// <param name="apiResourceNames">The names.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Models.ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null)
                throw new ArgumentNullException(nameof(apiResourceNames));

            //var query =
            //    from apiResource in Context.ApiResources
            //    where apiResourceNames.Contains(apiResource.Name)
            //    select apiResource;

            //var apis = query
            //    .Include(x => x.Secrets)
            //    .Include(x => x.Scopes)
            //    .Include(x => x.UserClaims)
            //    .Include(x => x.Properties)
            //    .AsNoTracking();

            //var result = (await apis.ToArrayAsync())
            //    .Where(x => apiResourceNames.Contains(x.Name))
            //    .Select(x => x.ToModel()).ToArray();

            List<ApiResource> resources = await Context.ApiResources.Where(p => apiResourceNames.Contains(p.Name))
                .IncludeMany(x => x.Secrets.Where(q => q.ApiResourceId == x.Id))
                .IncludeMany(x => x.Scopes.Where(q => q.ApiResourceId == x.Id))
                .IncludeMany(x => x.UserClaims.Where(q => q.ApiResourceId == x.Id))
                .IncludeMany(x => x.Properties.Where(q => q.ApiResourceId == x.Id))
                .NoTracking()
                .ToListAsync();
            if (resources == null)
                return new List<Models.ApiResource>();

            var result = resources.Select(x => x.ToModel()).ToList();
            if (result.Any())
            {
                Logger.LogDebug("Found {apis} API resource in database", result.Select(x => x.Name));
            }
            else
            {
                Logger.LogDebug("Did not find {apis} API resource in database", apiResourceNames);
            }
            return result;
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Models.ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var resources = await Context.ApiResources
                .Where(p => p.Scopes.AsSelect().Any(x => scopeNames.Contains(x.Scope) && p.Id == x.ApiResourceId))
                .IncludeMany(x => x.Secrets.Where(q => q.ApiResourceId == x.Id))
                .IncludeMany(x => x.Scopes.Where(q => q.ApiResourceId == x.Id))
                .IncludeMany(x => x.UserClaims.Where(q => q.ApiResourceId == x.Id))
                .IncludeMany(x => x.Properties.Where(q => q.ApiResourceId == x.Id))
                .NoTracking()
                .ToListAsync();
            if (resources == null)
                return new List<Models.ApiResource>();

            var models = resources.Select(x => x.ToModel()).ToList();
            return models;
        }


        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Models.IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var resources = await Context.IdentityResources.Where(p => scopeNames.Contains(p.Name))
                .IncludeMany(x => x.UserClaims.Where(q => q.IdentityResourceId == x.Id))
                .IncludeMany(x => x.Properties.Where(q => q.IdentityResourceId == x.Id))
                .NoTracking()
                .ToListAsync();
            if (resources == null)
                return new List<Models.IdentityResource>();

            return resources.Select(x => x.ToModel()).ToArray();
        }


        /// <summary>
        /// Gets scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Models.ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var resources = await Context.ApiScopes
                .Where(p => scopeNames.Contains(p.Name))
                .IncludeMany(x => x.UserClaims.Where(q => q.ScopeId == x.Id))
                .IncludeMany(x => x.Properties.Where(q => q.ScopeId == x.Id))
                .NoTracking()
                .ToListAsync();
            if (resources == null)
                return new List<Models.ApiScope>();

            return resources.Select(x => x.ToModel()).ToArray();
        }


        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Models.Resources> GetAllResourcesAsync()
        {
            var result = await _cache.GetOrCreateAsync(StorageCaches.CLIENT_ALLRESOURCES, async (entry) =>
            {
                //滑动过期，5分钟
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                var identity = Context.IdentityResources.Select
                    .IncludeMany(x => x.UserClaims.Where(q => q.IdentityResourceId == x.Id))
                    .IncludeMany(x => x.Properties.Where(q => q.IdentityResourceId == x.Id))
                    .NoTracking();

                var apis = Context.ApiResources.Select
                    .IncludeMany(x => x.Secrets.Where(q => q.ApiResourceId == x.Id))
                    .IncludeMany(x => x.Scopes.Where(q => q.ApiResourceId == x.Id))
                    .IncludeMany(x => x.UserClaims.Where(q => q.ApiResourceId == x.Id))
                    .IncludeMany(x => x.Properties.Where(q => q.ApiResourceId == x.Id))
                    .NoTracking();

                var scopes = Context.ApiScopes.Select
                    .IncludeMany(x => x.UserClaims.Where(q => q.ScopeId == x.Id))
                    .IncludeMany(x => x.Properties.Where(q => q.ScopeId == x.Id))
                    .NoTracking();

                var result = new Models.Resources(
                    (await identity.ToListAsync()).Select(x => x.ToModel()),
                    (await apis.ToListAsync()).Select(x => x.ToModel()),
                    (await scopes.ToListAsync()).Select(x => x.ToModel())
                );
                return result;
            });
            return result;
        }
    }
}