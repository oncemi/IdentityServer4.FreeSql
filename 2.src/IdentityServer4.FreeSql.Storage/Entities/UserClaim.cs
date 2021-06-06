// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


#pragma warning disable 1591

using FreeSql.DataAnnotations;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    public abstract class UserClaim : IEntity<long>
    {
        [Column(StringLength = 200, IsNullable = false)]
        public string Type { get; set; }
    }
}