using FreeSql.DataAnnotations;
using System;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    /// <summary>
    /// Entity for device flow codes
    /// </summary>
    [Table(Name = "ids_device_flow_codes")]
    [Index("index_{TableName}_" + nameof(Expiration), nameof(Expiration), false)]
    [Index("index_{TableName}_" + nameof(DeviceCode), nameof(DeviceCode), true)]
    public class DeviceFlowCodes
    {
        /// <summary>
        /// Gets or sets the user code.
        /// </summary>
        /// <value>
        /// The user code.
        /// </value>
        [Column(IsPrimary = true, StringLength = 255, IsNullable = false)]
        public string UserCode { get; set; }

        /// <summary>
        /// Gets or sets the device code.
        /// </summary>
        /// <value>
        /// The device code.
        /// </value>
        [Column(StringLength = 255, IsNullable = false)]
        public string DeviceCode { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        [Column(StringLength = 255, IsNullable = true)]
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        [Column(StringLength = 100, IsNullable = true)]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        [Column(StringLength = 255, IsNullable = false)]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets the description the user assigned to the device being authorized.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Column(StringLength = 255, IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>
        /// The creation time.
        /// </value>
        [Column(IsNullable = false)]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>
        /// The expiration.
        /// </value>
        [Column(IsNullable = false)]
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [Column(DbType = "text", IsNullable = true)]
        public string Data { get; set; }
    }
}