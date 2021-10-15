using FreeSql;
using IdentityServer4.FreeSql.Storage.Entities;
using IdentityServer4.FreeSql.Storage.Interfaces;
using IdentityServer4.FreeSql.Storage.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IdentityServer4.FreeSql.Storage.DbContexts
{

    /// <summary>
    /// DbContext for the IdentityServer configuration data.
    /// </summary>
    /// <seealso cref="FreeSql.DbContext" />
    /// <seealso cref="IdentityServer4.FreeSql.Interfaces.IConfigurationDbContext" />
    public class ConfigurationDbContext : ConfigurationDbContext<ConfigurationDbContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="ArgumentNullException">storeOptions</exception>
        public ConfigurationDbContext(IFreeSql<ConfigurationDbContext> freeSql, ConfigurationStoreOptions storeOptions)
            : base(freeSql, storeOptions)
        {
        }
    }

    /// <summary>
    /// DbContext for the IdentityServer configuration data.
    /// </summary>
    /// <seealso cref="Free.DbContext" />
    /// <seealso cref="IdentityServer4.EntityFramework.Interfaces.IConfigurationDbContext" />
    public class ConfigurationDbContext<TContext> : DbContext, IConfigurationDbContext
        where TContext : DbContext, IConfigurationDbContext
    {
        private readonly IFreeSql<ConfigurationDbContext> _freeSql;
        private readonly ConfigurationStoreOptions _storeOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="ArgumentNullException">storeOptions</exception>
        public ConfigurationDbContext(IFreeSql<ConfigurationDbContext> freeSql, ConfigurationStoreOptions storeOptions)
            : base(freeSql, null)
        {
            this._freeSql = freeSql ?? throw new ArgumentNullException(nameof(IFreeSql<ConfigurationDbContext>));
            this._storeOptions = storeOptions ?? throw new ArgumentNullException(nameof(ConfigurationStoreOptions));

            OnModelCreating(_freeSql.CodeFirst);
        }

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
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected new void OnModelCreating(ICodeFirst codefirst)
        {
            #region Config Entity

            codefirst.Entity<ApiResourceClaim>(entity =>
            {
                entity.ToTable(_storeOptions.ApiResourceClaim.Name);
            });
            codefirst.Entity<ApiResourceProperty>(entity =>
            {
                entity.ToTable(_storeOptions.ApiResourceProperty.Name);
            });
            codefirst.Entity<ApiResource>(entity =>
            {
                entity.ToTable(_storeOptions.ApiResource.Name);
            });
            codefirst.Entity<ApiResourceScope>(entity =>
            {
                entity.ToTable(_storeOptions.ApiResourceScope.Name);
            });
            codefirst.Entity<ApiResourceSecret>(entity =>
            {
                entity.ToTable(_storeOptions.ApiResourceSecret.Name);
            });
            codefirst.Entity<ApiScopeClaim>(entity =>
            {
                entity.ToTable(_storeOptions.ApiScopeClaim.Name);
            });
            codefirst.Entity<ApiScopeProperty>(entity =>
            {
                entity.ToTable(_storeOptions.ApiScopeProperty.Name);
            });
            codefirst.Entity<ApiScope>(entity =>
            {
                entity.ToTable(_storeOptions.ApiScope.Name);
            });
            codefirst.Entity<ClientCorsOrigin>(entity =>
            {
                entity.ToTable(_storeOptions.ClientCorsOrigin.Name);
            });
            codefirst.Entity<Client>(entity =>
            {
                entity.ToTable(_storeOptions.Client.Name);
            });
            codefirst.Entity<ClientClaim>(entity =>
            {
                entity.ToTable(_storeOptions.ClientClaim.Name);
            });
            codefirst.Entity<ClientGrantType>(entity =>
            {
                entity.ToTable(_storeOptions.ClientGrantType.Name);
            });
            codefirst.Entity<ClientIdPRestriction>(entity =>
            {
                entity.ToTable(_storeOptions.ClientIdPRestriction.Name);
            });
            codefirst.Entity<ClientPostLogoutRedirectUri>(entity =>
            {
                entity.ToTable(_storeOptions.ClientPostLogoutRedirectUri.Name);
            });
            codefirst.Entity<ClientProperty>(entity =>
            {
                entity.ToTable(_storeOptions.ClientProperty.Name);
            });
            codefirst.Entity<ClientRedirectUri>(entity =>
            {
                entity.ToTable(_storeOptions.ClientRedirectUri.Name);
            });
            codefirst.Entity<ClientScope>(entity =>
            {
                entity.ToTable(_storeOptions.ClientScopes.Name);
            });
            codefirst.Entity<ClientSecret>(entity =>
            {
                entity.ToTable(_storeOptions.ClientSecret.Name);
            });
            codefirst.Entity<IdentityResourceClaim>(entity =>
            {
                entity.ToTable(_storeOptions.IdentityResourceClaim.Name);
            });
            codefirst.Entity<IdentityResourceProperty>(entity =>
            {
                entity.ToTable(_storeOptions.IdentityResourceProperty.Name);
            });
            codefirst.Entity<IdentityResource>(entity =>
            {
                entity.ToTable(_storeOptions.IdentityResource.Name);
            });

            #endregion

            #region Create Table

            if (codefirst.IsAutoSyncStructure)
            {
                var items = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.FullName.StartsWith(typeof(DbSet<object>).FullName.Split('`')[0])).ToList();
                List<Type> entityTypes = new List<Type>();
                foreach (var item in items)
                {
                    if (item.PropertyType.GenericTypeArguments == null
                        || item.PropertyType.GenericTypeArguments.Length != 1)
                        continue;
                    entityTypes.Add(item.PropertyType.GenericTypeArguments[0]);
                }
                codefirst.SyncStructure(entityTypes.ToArray());
            }

            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseFreeSql(orm: _freeSql);
            base.OnConfiguring(builder);
        }

    }

}
