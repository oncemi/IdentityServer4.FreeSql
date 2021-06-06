// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


#pragma warning disable 1591

using FreeSql.DataAnnotations;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    [Index("index_{TableName}_" + nameof(ClientId), nameof(ClientId), true)]
    public class Client : IEntity<long>
    {
        public bool Enabled { get; set; } = true;

        [Column(StringLength = 200, IsNullable = false)]
        public string ClientId { get; set; }

        [Column(StringLength = 200, IsNullable = false)]
        public string ProtocolType { get; set; } = "oidc";

        [Navigate(nameof(ClientSecret.ClientId))]
        public List<ClientSecret> ClientSecrets { get; set; }

        public bool RequireClientSecret { get; set; } = true;

        [Column(StringLength = 200, IsNullable = true)]
        public string ClientName { get; set; }

        [Column(StringLength = 1000, IsNullable = true)]
        public string Description { get; set; }

        [Column(DbType = "text", IsNullable = true)]
        public string ClientUri { get; set; }

        [Column(DbType = "text", IsNullable = true)]
        public string LogoUri { get; set; }

        public bool RequireConsent { get; set; } = false;

        public bool AllowRememberConsent { get; set; } = true;

        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        [Navigate(nameof(ClientGrantType.ClientId))]
        public List<ClientGrantType> AllowedGrantTypes { get; set; }

        public bool RequirePkce { get; set; } = true;

        public bool AllowPlainTextPkce { get; set; }

        public bool RequireRequestObject { get; set; }

        public bool AllowAccessTokensViaBrowser { get; set; }

        [Navigate(nameof(ClientRedirectUri.ClientId))]
        public List<ClientRedirectUri> RedirectUris { get; set; }

        [Navigate(nameof(ClientPostLogoutRedirectUri.ClientId))]
        public List<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }

        [Column(DbType = "text", IsNullable = true)]
        public string FrontChannelLogoutUri { get; set; }

        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

        [Column(DbType = "text", IsNullable = true)]
        public string BackChannelLogoutUri { get; set; }

        public bool BackChannelLogoutSessionRequired { get; set; } = true;
        
        public bool AllowOfflineAccess { get; set; }

        [Navigate(nameof(ClientScope.ClientId))]
        public List<ClientScope> AllowedScopes { get; set; }

        public int IdentityTokenLifetime { get; set; } = 300;

        [Column(StringLength = 100, IsNullable = true)]
        public string AllowedIdentityTokenSigningAlgorithms { get; set; }
        
        public int AccessTokenLifetime { get; set; } = 3600;
        
        public int AuthorizationCodeLifetime { get; set; } = 300;

        [Column(IsNullable = true)]
        public int? ConsentLifetime { get; set; } = null;
        
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;
        
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;
        
        public int AccessTokenType { get; set; } = (int)0; // AccessTokenType.Jwt;
        
        public bool EnableLocalLogin { get; set; } = true;

        [Navigate(nameof(ClientIdPRestriction.ClientId))]
        public List<ClientIdPRestriction> IdentityProviderRestrictions { get; set; }
        
        public bool IncludeJwtId { get; set; }

        [Navigate(nameof(ClientClaim.ClientId))]
        public List<ClientClaim> Claims { get; set; }

        public bool AlwaysSendClientClaims { get; set; }

        [Column(StringLength = 200, IsNullable = true)]
        public string ClientClaimsPrefix { get; set; } = "client_";

        [Column(StringLength = 200, IsNullable = true)]
        public string PairWiseSubjectSalt { get; set; }

        [Navigate(nameof(ClientCorsOrigin.ClientId))]
        public List<ClientCorsOrigin> AllowedCorsOrigins { get; set; }

        [Navigate(nameof(ClientProperty.ClientId))]
        public List<ClientProperty> Properties { get; set; }

        [Column(IsNullable = true, ServerTime = DateTimeKind.Utc)]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Column(IsNullable = true)]
        public DateTime? Updated { get; set; }

        [Column(IsNullable = true)]
        public DateTime? LastAccessed { get; set; }

        [Column(IsNullable = true)]
        public int? UserSsoLifetime { get; set; }

        [Column(StringLength = 100, IsNullable = true)]
        public string UserCodeType { get; set; }

        public int DeviceCodeLifetime { get; set; } = 300;

        public bool NonEditable { get; set; }
    }
}