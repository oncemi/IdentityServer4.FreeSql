﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


#pragma warning disable 1591

using FreeSql.DataAnnotations;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    [Table(Name = "ids_api_scope_properties")]
    public class ApiScopeProperty : Property
    {
        public long ScopeId { get; set; }

        [Navigate(nameof(ApiScope.Id))]
        public ApiScope Scope { get; set; }
    }
}