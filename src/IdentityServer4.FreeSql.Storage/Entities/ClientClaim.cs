// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


#pragma warning disable 1591

using FreeSql.DataAnnotations;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    [Table(Name = "ids_client_claims")]
    public class ClientClaim : IEntity<long>
    {
        [Column(StringLength = 255, IsNullable = false)]
        public string Type { get; set; }

        [Column(StringLength = 255, IsNullable = false)]
        public string Value { get; set; }

        public long ClientId { get; set; }

        [Navigate(nameof(Entities.Client.ClientId))]
        public Client Client { get; set; }
    }
}