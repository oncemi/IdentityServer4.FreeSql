// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


#pragma warning disable 1591

using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    [Table(Name = "ids_identity_resources")]
    [Index("index_{TableName}_" + nameof(Name), nameof(Name), true)]
    public class IdentityResource : IEntity<long>
    {
        public bool Enabled { get; set; } = true;

        [Column(StringLength = 255, IsNullable = false)]
        public string Name { get; set; }

        [Column(StringLength = 255, IsNullable = true)]
        public string DisplayName { get; set; }

        [Column(DbType = "text", IsNullable = true)]
        public string Description { get; set; }

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public bool ShowInDiscoveryDocument { get; set; } = true;

        [Navigate(nameof(IdentityResourceClaim.IdentityResourceId))]
        public List<IdentityResourceClaim> UserClaims { get; set; }

        [Navigate(nameof(IdentityResourceProperty.IdentityResourceId))]
        public List<IdentityResourceProperty> Properties { get; set; }

        [Column(IsNullable = true, ServerTime = DateTimeKind.Utc)]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Column(IsNullable = true)]
        public DateTime? Updated { get; set; }

        public bool NonEditable { get; set; }
    }
}
