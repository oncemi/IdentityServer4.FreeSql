using System;
using System.Threading.Tasks;
using IdentityServer4.FreeSql.Storage.Entities;
using FreeSql;

namespace IdentityServer4.FreeSql.Storage.Interfaces
{
    /// <summary>
    /// Abstraction for the operational data context.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IPersistedGrantDbContext : IDisposable
    {
        /// <summary>
        /// Gets or sets the persisted grants.
        /// </summary>
        /// <value>
        /// The persisted grants.
        /// </value>
        DbSet<PersistedGrant> PersistedGrants { get; set; }

        /// <summary>
        /// Gets or sets the device flow codes.
        /// </summary>
        /// <value>
        /// The device flow codes.
        /// </value>
        DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

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
