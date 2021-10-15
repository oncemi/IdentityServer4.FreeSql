// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


#pragma warning disable 1591

using FreeSql.DataAnnotations;
using System;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    [Index("index_{TableName}_" + nameof(SubjectId), nameof(SubjectId), false)]
    [Index("index_{TableName}_" + nameof(ClientId), nameof(ClientId), false)]
    [Index("index_{TableName}_" + nameof(Type), nameof(Type), false)]
    public class PersistedGrant
    {
        [Column(IsPrimary = true, StringLength = 255, IsNullable = false)]
        public string Key { get; set; }

        [Column(StringLength = 50, IsNullable = false)]
        public string Type { get; set; }

        [Column(StringLength = 255, IsNullable = true)]
        public string SubjectId { get; set; }

        [Column(StringLength = 100, IsNullable = true)]
        public string SessionId { get; set; }

        [Column(StringLength = 255, IsNullable = false)]
        public string ClientId { get; set; }

        [Column(StringLength = 255, IsNullable = true)]
        public string Description { get; set; }

        [Column(IsNullable = false)]
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;

        [Column(IsNullable = true)]
        public DateTime? Expiration { get; set; }

        [Column(IsNullable = false)]
        public DateTime? ConsumedTime { get; set; }

        [Column(DbType = "text", IsNullable = true)]
        public string Data { get; set; }
    }
}