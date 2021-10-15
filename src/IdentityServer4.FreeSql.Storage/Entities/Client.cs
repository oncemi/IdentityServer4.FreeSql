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
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 客户端Id
        /// </summary>
        [Column(StringLength = 200, IsNullable = false)]
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端类型（默认：oidc）
        /// </summary>
        [Column(StringLength = 200, IsNullable = false)]
        public string ProtocolType { get; set; } = "oidc";

        /// <summary>
        /// 客户端密钥
        /// </summary>
        [Navigate(nameof(ClientSecret.ClientId))]
        public List<ClientSecret> ClientSecrets { get; set; }

        /// <summary>
        /// 是否需要客户端密钥
        /// </summary>
        public bool RequireClientSecret { get; set; } = true;

        /// <summary>
        /// 客户端名称
        /// </summary>
        [Column(StringLength = 200, IsNullable = true)]
        public string ClientName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Column(StringLength = 1000, IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        [Column(DbType = "text", IsNullable = true)]
        public string ClientUri { get; set; }
        
        /// <summary>
        /// Logo地址
        /// </summary>
        [Column(DbType = "text", IsNullable = true)]
        public string LogoUri { get; set; }

        /// <summary>
        /// 是否需要同意授权
        /// </summary>
        public bool RequireConsent { get; set; } = false;

        /// <summary>
        /// 是否允许记住同意授权
        /// </summary>
        public bool AllowRememberConsent { get; set; } = true;

        /// <summary>
        /// 允许在IdToken中包含用户Claims
        /// </summary>
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        /// <summary>
        /// 允许的授权类型，exp:authorization_code
        /// </summary>
        [Navigate(nameof(ClientGrantType.ClientId))]
        public List<ClientGrantType> AllowedGrantTypes { get; set; }

        /// <summary>
        /// 是否需要PKCE
        /// </summary>
        public bool RequirePkce { get; set; } = true;

        /// <summary>
        /// 是否允许显式包含PKCE
        /// </summary>
        public bool AllowPlainTextPkce { get; set; }

        public bool RequireRequestObject { get; set; }

        public bool AllowAccessTokensViaBrowser { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        [Navigate(nameof(ClientRedirectUri.ClientId))]
        public List<ClientRedirectUri> RedirectUris { get; set; }

        /// <summary>
        /// POST登出跳出地址
        /// </summary>
        [Navigate(nameof(ClientPostLogoutRedirectUri.ClientId))]
        public List<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// 前置登出地址
        /// </summary>
        [Column(DbType = "text", IsNullable = true)]
        public string FrontChannelLogoutUri { get; set; }

        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

        /// <summary>
        /// 后置登出地址
        /// </summary>
        [Column(DbType = "text", IsNullable = true)]
        public string BackChannelLogoutUri { get; set; }

        public bool BackChannelLogoutSessionRequired { get; set; } = true;
        
        /// <summary>
        /// 是否允许离线访问
        /// </summary>
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
        
        public int AccessTokenType { get; set; } = (int)Models.AccessTokenType.Jwt;
        
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