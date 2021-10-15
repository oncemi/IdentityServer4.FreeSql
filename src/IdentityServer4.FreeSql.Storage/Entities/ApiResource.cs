// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#pragma warning disable 1591

using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    [Index("index_{TableName}_" + nameof(Name), nameof(Name), false)]
    public class ApiResource : IEntity<long>
    {
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        [Column(StringLength = 200, IsNullable = false)]
        public string Name { get; set; }

        [Column(StringLength = 200, IsNullable = true)]
        public string DisplayName { get; set; }

        [Column(StringLength = 200, IsNullable = true)]
        public string Description { get; set; }

        [Column(StringLength = 100, IsNullable = true)]
        public string AllowedAccessTokenSigningAlgorithms { get; set; }

        public bool ShowInDiscoveryDocument { get; set; } = true;

        [Column(IsNullable = true, ServerTime = DateTimeKind.Utc)]
        public DateTime Created { get; set; }

        [Column(IsNullable = true)]
        public DateTime? Updated { get; set; }

        [Column(IsNullable = true)]
        public DateTime? LastAccessed { get; set; }

        public bool NonEditable { get; set; }

        [Navigate(nameof(ApiResourceSecret.ApiResourceId))]
        public List<ApiResourceSecret> Secrets { get; set; }

        [Navigate(nameof(ApiResourceScope.ApiResourceId))]
        public List<ApiResourceScope> Scopes { get; set; }

        [Navigate(nameof(ApiResourceClaim.ApiResourceId))]
        public List<ApiResourceClaim> UserClaims { get; set; }

        [Navigate(nameof(ApiResourceProperty.ApiResourceId))]
        public List<ApiResourceProperty> Properties { get; set; }
    }
}
