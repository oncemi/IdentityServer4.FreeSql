using FreeSql;
using IdentityServer4.FreeSql.Storage.Interfaces;
using IdentityServer4.FreeSql.Storage.Mappers;
using IdentityServer4.FreeSql.Storage.Options;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.FreeSql.Storage.Stores
{
    public class ClientStore : IClientStore
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        protected readonly IConfigurationDbContext Context;

        /// <summary>
        /// 日志
        /// </summary>
        protected readonly ILogger<ClientStore> _logger;

        /// <summary>
        /// 缓存
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 初始化一个 <参阅 cref="ClientStore"/> 类的新实例.
        /// </summary>
        /// <param name="context">数据库上下文</param>
        /// <param name="logger">日志</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public ClientStore(IConfigurationDbContext context
            , IMemoryCache cache
            , ILogger<ClientStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(paramName: nameof(context));
            _cache = cache ?? throw new ArgumentNullException(paramName: nameof(cache));
            _logger = logger;
        }

        /// <summary>
        /// 通过客户端标识查找客户端
        /// </summary>
        /// <param name="clientId">客户端标识</param>
        /// <returns>客户端</returns>
        public virtual async Task<Client> FindClientByIdAsync(string clientId)
        {
            //使用缓存
            var model = await _cache.GetOrCreateAsync(StorageCaches.GetClientCacheKey(clientId), async (entry) =>
             {
                 //滑动过期，3分钟
                 entry.SlidingExpiration = TimeSpan.FromMinutes(3);
                 //get client
                 Entities.Client client = await Context.Clients
                      .Where(p => p.ClientId == clientId)
                      .IncludeMany(c => c.AllowedCorsOrigins.Where(p => p.ClientId == c.Id))
                      .IncludeMany(c => c.AllowedGrantTypes.Where(p => p.ClientId == c.Id))
                      .IncludeMany(c => c.AllowedScopes.Where(p => p.ClientId == c.Id))
                      .IncludeMany(c => c.Claims.Where(p => p.ClientId == c.Id))
                      .IncludeMany(c => c.ClientSecrets.Where(p => p.ClientId == c.Id))
                      .IncludeMany(c => c.IdentityProviderRestrictions.Where(p => p.ClientId == c.Id))
                      .IncludeMany(c => c.PostLogoutRedirectUris.Where(p => p.ClientId == c.Id))
                      .IncludeMany(c => c.Properties.Where(p => p.ClientId == c.Id))
                      .IncludeMany(c => c.RedirectUris.Where(p => p.ClientId == c.Id))
                      .ToOneAsync();
                 if (client == null)
                     return null;
                 var model = client.ToModel();
                 return model;
             });
            if (model == null)
            {
                _logger.LogWarning($"Query client by clientId '{clientId}' failed. ");
            }
            else
            {
                _logger.LogInformation($"Query client by clientId '{clientId}' successed. ");
            }
            return model;
        }
    }
}
