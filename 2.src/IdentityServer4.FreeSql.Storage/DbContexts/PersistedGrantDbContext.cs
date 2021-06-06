using System;
using System.Threading.Tasks;
using IdentityServer4.FreeSql.Storage.Entities;
using IdentityServer4.FreeSql.Storage.Interfaces;
using IdentityServer4.FreeSql.Storage.Options;
using FreeSql;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.FreeSql.Storage.DbContexts
{
    /// <summary>
    /// DbContext for the IdentityServer operational data.
    /// </summary>
    /// <seealso cref="FreeSql.DbContext" />
    /// <seealso cref="IdentityServer4.FreeSql.Interfaces.IPersistedGrantDbContext" />
    public class PersistedGrantDbContext : PersistedGrantDbContext<PersistedGrantDbContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="ArgumentNullException">storeOptions</exception>
        public PersistedGrantDbContext(IFreeSql<PersistedGrantDbContext> freeSql, OperationalStoreOptions storeOptions)
            : base(freeSql, storeOptions)
        {
        }
    }

    /// <summary>
    /// DbContext for the IdentityServer operational data.
    /// </summary>
    /// <seealso cref="FreeSql.DbContext" />
    /// <seealso cref="IdentityServer4.FreeSql.Interfaces.IPersistedGrantDbContext" />
    public class PersistedGrantDbContext<TContext> : DbContext, IPersistedGrantDbContext
        where TContext : DbContext, IPersistedGrantDbContext
    {
        private readonly IFreeSql<PersistedGrantDbContext> _freeSql;
        private readonly OperationalStoreOptions _storeOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="ArgumentNullException">storeOptions</exception>
        public PersistedGrantDbContext(IFreeSql<PersistedGrantDbContext> freeSql, OperationalStoreOptions storeOptions)
            : base(freeSql, null)
        {
            this._freeSql = freeSql;
            if (storeOptions == null) throw new ArgumentNullException(nameof(storeOptions));
            this._storeOptions = storeOptions;

            OnModelCreating(_freeSql.CodeFirst);
        }

        /// <summary>
        /// Gets or sets the persisted grants.
        /// </summary>
        /// <value>
        /// The persisted grants.
        /// </value>
        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        /// <summary>
        /// Gets or sets the device codes.
        /// </summary>
        /// <value>
        /// The device codes.
        /// </value>
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

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
            codefirst.Entity<PersistedGrant>(entity =>
            {
                entity.ToTable(_storeOptions.PersistedGrants.Name);
            });
            codefirst.Entity<DeviceFlowCodes>(entity =>
            {
                entity.ToTable(_storeOptions.DeviceFlowCodes.Name);
            });

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
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseFreeSql(orm: _freeSql);
            base.OnConfiguring(builder);
        }
    }

}
