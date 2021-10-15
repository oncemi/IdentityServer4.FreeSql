using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreeSql;
using IdentityServer4.FreeSql.Storage.Entities;

namespace IdentityServer4.FreeSql.Storage.Interfaces
{
    /// <summary>
    /// 配置上下文的抽象
    /// </summary>
    /// <可参阅 cref="System.IDisposable">
    public interface IConfigurationDbContext : IDisposable
    {
        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public DbSet<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the clients' CORS origins.
        /// </summary>
        /// <value>
        /// The clients CORS origins.
        /// </value>
        public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }

        /// <summary>
        /// Gets or sets the identity resources.
        /// </summary>
        /// <value>
        /// The identity resources.
        /// </value>
        public DbSet<IdentityResource> IdentityResources { get; set; }

        /// <summary>
        /// Gets or sets the API resources.
        /// </summary>
        /// <value>
        /// The API resources.
        /// </value>
        public DbSet<ApiResource> ApiResources { get; set; }

        /// <summary>
        /// Gets or sets the API scopes.
        /// </summary>
        /// <value>
        /// The API resources.
        /// </value>
        public DbSet<ApiScope> ApiScopes { get; set; }

        public DbSet<ApiResourceSecret> ApiResourceSecrets { get; set; }

        public DbSet<ApiResourceScope> ApiResourceScopes { get; set; }

        public DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }

        public DbSet<ApiResourceProperty> ApiResourceProperties { get; set; }

        public DbSet<ApiScopeProperty> ApiScopeProperties { get; set; }

        public DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        public DbSet<ClientSecret> ClientSecrets { get; set; }

        public DbSet<ClientGrantType> ClientGrantTypes { get; set; }

        public DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }

        public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }

        public DbSet<ClientScope> ClientScopes { get; set; }

        public DbSet<ClientIdPRestriction> ClientIdPRestrictions { get; set; }

        public DbSet<ClientClaim> ClientClaims { get; set; }

        public DbSet<ClientProperty> ClientProperties { get; set; }

        public DbSet<IdentityResourceClaim> IdentityResourceClaims { get; set; }

        public DbSet<IdentityResourceProperty> IdentityResourceProperties { get; set; }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
    }
}
