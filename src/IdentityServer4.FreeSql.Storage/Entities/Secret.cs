// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


#pragma warning disable 1591

using FreeSql.DataAnnotations;
using System;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    public abstract class Secret : IEntity<long>
    {
        [Column(StringLength = 1000, IsNullable = true)]
        public string Description { get; set; }

        [Column(StringLength = 4000, IsNullable = false)]
        public string Value { get; set; }

        [Column(IsNullable = true)]
        public DateTime? Expiration { get; set; }

        [Column(StringLength = 255, IsNullable = false)]
        public string Type { get; set; } = "SharedSecret";

        [Column(IsNullable = false)]
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}