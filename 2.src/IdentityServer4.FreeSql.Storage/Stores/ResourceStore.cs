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
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public ResourceStore(IConfigurationDbContext context, ILogger<ResourceStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
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

            //List<ApiResource> apiResources = await Context.ApiResources.Where(p => apiResourceNames.Contains(p.Name)).ToListAsync();
            //if(apiResources == null || apiResources.Count == 0)
            //{
            //    return new List<Models.ApiResource>();
            //}
            //List<long> ids = apiResources.Select(p => p.Id).ToList();

            //var secrets = await Context.ApiResourceSecret.Where(p => ids.Contains(p.ApiResourceId)).ToListAsync();
            //var scopes = await Context.ApiResourceScope.Where(p => ids.Contains(p.ApiResourceId)).ToListAsync();
            //var userClaims = await Context.ApiResourceClaim.Where(p => ids.Contains(p.ApiResourceId)).ToListAsync();
            //var properties = await Context.ApiResourceProperty.Where(p => ids.Contains(p.ApiResourceId)).ToListAsync();

            //foreach(var item in apiResources)
            //{
            //    item.Secrets = secrets == null ? new List<ApiResourceSecret>() : secrets.Where(p => p.ApiResourceId == item.Id).ToList();
            //    item.Scopes = scopes == null ? new List<ApiResourceScope>() : scopes.Where(p => p.ApiResourceId == item.Id).ToList();
            //    item.UserClaims = userClaims == null ? new List<ApiResourceClaim>() : userClaims.Where(p => p.ApiResourceId == item.Id).ToList();
            //    item.Properties = properties == null ? new List<ApiResourceProperty>() : properties.Where(p => p.ApiResourceId == item.Id).ToList();
            //}
            //var result = apiResources.Select(x => x.ToModel()).ToList();

            //var query =
            //  from apiResource in Context.ApiResources
            //  where apiResourceNames.Contains(apiResource.Name)
            //  select apiResource;

            //var apis = query
            //    .IncludeMany(x => x.Secrets)
            //    .IncludeMany(x => x.Scopes)
            //    .IncludeMany(x => x.UserClaims)
            //    .IncludeMany(x => x.Properties)
            //    .NoTracking();

            //var result = (await apis.ToListAsync())
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
        }
    }
}