using IdentityModel;
using IdentityServer4;
using IdentityServer4.FreeSql.Storage.Entities;
using IdentityServer4.FreeSql.Storage.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnceMi.AspNetCore.IdGenerator;
using OnceMi.IdentityServer4.User;
using System.Collections.Generic;
using System.Linq;

namespace OnceMi.IdentityServer4.Extensions
{
    public static class InitializeDatabaseService
    {
        public static IApplicationBuilder UseInitializeDatabase(this IApplicationBuilder app)
        {
            IWebHostEnvironment env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            if (!env.IsDevelopment())
            {
                return app;
            }
            using (var scope = app.ApplicationServices.CreateScope())
            {
                ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                IIdGeneratorService idGenerator = scope.ServiceProvider.GetRequiredService<IIdGeneratorService>();
                IConfigurationDbContext configurationDbContext = scope.ServiceProvider.GetRequiredService<IConfigurationDbContext>();
                IPersistedGrantDbContext persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<IPersistedGrantDbContext>();
                UserDbContext userDbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

                logger.LogInformation("Start seed database...");

                #region client

                List<IdentityResource> resources = new List<IdentityResource>()
                {
                    new IdentityResource()
                    {
                        Id = idGenerator.NewId(),
                        Name = JwtClaimTypes.Role,
                        Enabled = true,
                        Required = false,
                        DisplayName = "角色信息",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Role,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Id = idGenerator.NewId(),
                        Name = OrganizeJwtClaimType.Organize,
                        Enabled = true,
                        Required = false,
                        DisplayName = "组织信息",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = OrganizeJwtClaimType.Organize,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Id = idGenerator.NewId(),
                        Name = IdentityServerConstants.StandardScopes.OpenId,
                        Enabled = true,
                        Required = true,
                        DisplayName = "用户Id",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Subject,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Id = idGenerator.NewId(),
                        Name = IdentityServerConstants.StandardScopes.Profile,
                        Enabled = true,
                        Required = true,
                        DisplayName = "基本信息",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Name,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.FamilyName,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.GivenName,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.MiddleName,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.NickName,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.PreferredUserName,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Profile,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Picture,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Gender,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.WebSite,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Id = idGenerator.NewId(),
                        Name = IdentityServerConstants.StandardScopes.Email,
                        Enabled = true,
                        Required = false,
                        DisplayName = "邮箱",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Email,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.EmailVerified,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Id = idGenerator.NewId(),
                        Name = IdentityServerConstants.StandardScopes.Address,
                        Enabled = true,
                        Required = false,
                        DisplayName = "地址",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Address,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Id = idGenerator.NewId(),
                        Name = IdentityServerConstants.StandardScopes.Phone,
                        Enabled = true,
                        Required = false,
                        DisplayName = "电话号码",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.PhoneNumber,
                            },
                            new IdentityResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.PhoneNumberVerified,
                            }
                        }
                    },
                };

                List<ApiScope> apiScopes = new List<ApiScope>()
                {
                    new ApiScope()
                    {
                        Id = idGenerator.NewId(),
                        Name = "api1",
                        DisplayName = "api1",
                        UserClaims = new List<ApiScopeClaim>(),
                    },
                    new ApiScope()
                    {
                        Id = idGenerator.NewId(),
                        Name = "client_management",
                        DisplayName = "client_management",
                        UserClaims = new List<ApiScopeClaim>(),
                    },
                };

                List<ApiResource> apiResources = new List<ApiResource>()
                {
                    new ApiResource()
                    {
                        Id = idGenerator.NewId(),
                        Name = "api1",
                        DisplayName = "api1",
                        Scopes = new List<ApiResourceScope>()
                        {
                            new ApiResourceScope()
                            {
                                Id = idGenerator.NewId(),
                                Scope = apiScopes.Where(p=>p.DisplayName == "api1").FirstOrDefault()?.Name
                            }
                        },
                        UserClaims = new List<ApiResourceClaim>()
                        {
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Subject,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Profile,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Picture,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Name,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.NickName,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Role,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = OrganizeJwtClaimType.Organize,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.Email,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.EmailVerified,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.PhoneNumber,
                            },
                            new ApiResourceClaim()
                            {
                                Id = idGenerator.NewId(),
                                Type = JwtClaimTypes.PhoneNumberVerified,
                            },
                        }
                    }
                };

                List<Client> clients = new List<Client>()
                {
                    new Client()
                    {
                        Id = idGenerator.NewId(),
                        ClientId = "5954398486",
                        ClientName = "后台管理系统Vue客户端",
                        ClientUri = "",
                        RequireClientSecret = false,
                        RequireConsent = false,
                        AllowRememberConsent = true,
                        AllowPlainTextPkce = false,
                        AllowAccessTokensViaBrowser = true,
                        BackChannelLogoutSessionRequired = true,
                        AllowOfflineAccess = true,
                        UpdateAccessTokenClaimsOnRefresh = true,
                        AccessTokenType = 0,
                        AllowedCorsOrigins = new List<ClientCorsOrigin>()
                        {
                            new ClientCorsOrigin()
                            {
                                Id = idGenerator.NewId(),
                                Origin = "http://localhost:8080",
                            }
                        },
                        AllowedGrantTypes = new List<ClientGrantType>()
                        {
                            new ClientGrantType()
                            {
                                Id = idGenerator.NewId(),
                                GrantType = "authorization_code",
                            }
                        },
                        RequirePkce = true,
                        ClientSecrets = new List<ClientSecret>()
                        {
                            new ClientSecret()
                            {
                                Id = idGenerator.NewId(),
                                Value = "oYjppIizd29W4eodalgf+Vry0MfyLunBPVZeFmOelvU="
                            }
                        },
                        RedirectUris = new List<ClientRedirectUri>()
                        {
                            new ClientRedirectUri()
                            {
                                Id = idGenerator.NewId(),
                                RedirectUri = "http://localhost:8080/#/callback"
                            },
                            new ClientRedirectUri()
                            {
                                Id = idGenerator.NewId(),
                                RedirectUri = "http://localhost:8080/#/refresh"
                            }
                        },
                        FrontChannelLogoutUri = "http://localhost:8080",
                        PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>()
                        {
                            new ClientPostLogoutRedirectUri()
                            {
                                Id = idGenerator.NewId(),
                                PostLogoutRedirectUri = "http://localhost:8080",
                            }
                        },
                        AllowedScopes = new List<ClientScope>()
                        {
                            new ClientScope()
                            {
                                Id = idGenerator.NewId(),
                                Scope = JwtClaimTypes.Subject,
                            },
                            new ClientScope()
                            {
                                Id = idGenerator.NewId(),
                                Scope = "openid",
                            },
                            new ClientScope()
                            {
                                Id = idGenerator.NewId(),
                                Scope = JwtClaimTypes.Email,
                            },
                            new ClientScope()
                            {
                                Id = idGenerator.NewId(),
                                Scope = JwtClaimTypes.PhoneNumber,
                            },
                            new ClientScope()
                            {
                                Id = idGenerator.NewId(),
                                Scope = JwtClaimTypes.Profile,
                            },
                            new ClientScope()
                            {
                                Id = idGenerator.NewId(),
                                Scope = JwtClaimTypes.Role,
                            },
                            new ClientScope()
                            {
                                Id = idGenerator.NewId(),
                                Scope = OrganizeJwtClaimType.Organize,
                            },
                            new ClientScope()
                            {
                                Id = idGenerator.NewId(),
                                Scope = "api1",
                            },
                        }
                    }
                };

                if (configurationDbContext.IdentityResources.Select.Count() == 0)
                {
                    foreach (var item in resources)
                    {
                        if (item.UserClaims != null && item.UserClaims.Count > 0)
                        {
                            foreach (var q in item.UserClaims)
                            {
                                q.IdentityResourceId = item.Id;
                                configurationDbContext.IdentityResourceClaims.Add(q);
                            }
                        }
                        configurationDbContext.IdentityResources.Add(item);
                    }
                }

                if (configurationDbContext.ApiScopes.Select.Count() == 0)
                {
                    foreach (var item in apiScopes)
                    {
                        if (item.UserClaims != null && item.UserClaims.Count > 0)
                        {
                            foreach (var q in item.UserClaims)
                            {
                                q.ScopeId = item.Id;
                                configurationDbContext.ApiScopeClaims.Add(q);
                            }
                        }
                        configurationDbContext.ApiScopes.Add(item);
                    }
                }

                if (configurationDbContext.ApiResources.Select.Count() == 0)
                {
                    foreach (var item in apiResources)
                    {
                        if (item.Scopes != null && item.Scopes.Count > 0)
                        {
                            foreach (var q in item.Scopes)
                            {
                                q.ApiResourceId = item.Id;
                                configurationDbContext.ApiResourceScopes.Add(q);
                            }
                        }
                        if (item.UserClaims != null && item.UserClaims.Count > 0)
                        {
                            foreach (var q in item.UserClaims)
                            {
                                q.ApiResourceId = item.Id;
                                configurationDbContext.ApiResourceClaims.Add(q);
                            }
                        }
                        configurationDbContext.ApiResources.Add(item);
                    }
                }

                if (configurationDbContext.Clients.Select.Count() == 0)
                {
                    logger.LogInformation("初始化客户端信息...");

                    foreach (var item in clients)
                    {
                        if (item.AllowedGrantTypes != null && item.AllowedGrantTypes.Count > 0)
                        {
                            foreach (var q in item.AllowedGrantTypes)
                            {
                                q.ClientId = item.Id;
                                configurationDbContext.ClientGrantTypes.Add(q);
                            }
                        }
                        if (item.ClientSecrets != null && item.ClientSecrets.Count > 0)
                        {
                            foreach (var q in item.ClientSecrets)
                            {
                                q.ClientId = item.Id;
                                configurationDbContext.ClientSecrets.Add(q);
                            }
                        }
                        if (item.RedirectUris != null && item.RedirectUris.Count > 0)
                        {
                            foreach (var q in item.RedirectUris)
                            {
                                q.ClientId = item.Id;
                                configurationDbContext.ClientRedirectUris.Add(q);
                            }
                        }
                        if (item.PostLogoutRedirectUris != null && item.PostLogoutRedirectUris.Count > 0)
                        {
                            foreach (var q in item.PostLogoutRedirectUris)
                            {
                                q.ClientId = item.Id;
                                configurationDbContext.ClientPostLogoutRedirectUris.Add(q);
                            }
                        }
                        if (item.AllowedCorsOrigins != null && item.AllowedCorsOrigins.Count > 0)
                        {
                            foreach (var q in item.AllowedCorsOrigins)
                            {
                                q.ClientId = item.Id;
                                configurationDbContext.ClientCorsOrigins.Add(q);
                            }
                        }
                        if (item.AllowedScopes != null && item.AllowedScopes.Count > 0)
                        {
                            foreach (var q in item.AllowedScopes)
                            {
                                q.ClientId = item.Id;
                                configurationDbContext.ClientScopes.Add(q);
                            }
                        }
                        configurationDbContext.Clients.Add(item);
                    }
                }

                #endregion

                configurationDbContext.SaveChanges();
            }
            return app;
        }
    }
}
