// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#pragma warning disable 1591

using FreeSql.DataAnnotations;
using System.Collections.Generic;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    public class ApiScope : IEntity<long>
    {
        public bool Enabled { get; set; } = true;

        [Column(StringLength = 200, IsNullable = false)]
        public string Name { get; set; }

        [Column(StringLength = 200, IsNullable = true)]
        public string DisplayName { get; set; }

        [Column(StringLength = 200, IsNullable = true)]
        public string Description { get; set; }

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public bool ShowInDiscoveryDocument { get; set; } = true;

        [Navigate(nameof(ApiScopeClaim.ScopeId))]
        public List<ApiScopeClaim> UserClaims { get; set; }

        [Navigate(nameof(ApiScopeProperty.ScopeId))]
        public List<ApiScopeProperty> Properties { get; set; }
    }
}