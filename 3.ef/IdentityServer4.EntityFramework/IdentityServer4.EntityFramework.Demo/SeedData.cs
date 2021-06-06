using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.EntityFramework.Demo
{
    public static class SeedData
    {
        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var persistedGrantDbContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                var configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                persistedGrantDbContext.Database.MigrateAsync();
                configurationDbContext.Database.MigrateAsync();

                List<IdentityResource> resources = new List<IdentityResource>()
                {
                    new IdentityResource()
                    {
                        Name = "roles",
                        Enabled = true,
                        Required = false,
                        DisplayName = "角色信息",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Type = JwtClaimTypes.Role,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Name = IdentityServerConstants.StandardScopes.OpenId,
                        Enabled = true,
                        Required = true,
                        DisplayName = "用户Id",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Type = JwtClaimTypes.Subject,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Name = IdentityServerConstants.StandardScopes.Profile,
                        Enabled = true,
                        Required = true,
                        DisplayName = "基本信息",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                //
                                Type = JwtClaimTypes.Name,
                            },
                            new IdentityResourceClaim()
                            {
                                //
                                Type = JwtClaimTypes.FamilyName,
                            },
                            new IdentityResourceClaim()
                            {
                                //
                                Type = JwtClaimTypes.GivenName,
                            },
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.MiddleName,
                            },
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.NickName,
                            },
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.PreferredUserName,
                            },
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.Profile,
                            },
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.Picture,
                            },
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.Gender,
                            },
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.UpdatedAt,
                            },
                        }
                    },
                    new IdentityResource()
                    {
                        Name = IdentityServerConstants.StandardScopes.Email,
                        Enabled = true,
                        Required = false,
                        DisplayName = "邮箱",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.Email,
                            },
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.EmailVerified,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Name = IdentityServerConstants.StandardScopes.Address,
                        Enabled = true,
                        Required = false,
                        DisplayName = "地址",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.Address,
                            }
                        }
                    },
                    new IdentityResource()
                    {
                        Name = IdentityServerConstants.StandardScopes.Phone,
                        Enabled = true,
                        Required = false,
                        DisplayName = "电话号码",
                        UserClaims = new List<IdentityResourceClaim>()
                        {
                            new IdentityResourceClaim()
                            {
                                Type = JwtClaimTypes.PhoneNumber,
                            },
                            new IdentityResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.PhoneNumberVerified,
                            }
                        }
                    },
                };

                List<ApiScope> apiScopes = new List<ApiScope>()
                {
                    new ApiScope()
                    {
                        Name = "api1",
                        DisplayName = "api1",
                        UserClaims = new List<ApiScopeClaim>(),
                    },
                    new ApiScope()
                    {
                        Name = "client_management",
                        DisplayName = "client_management",
                        UserClaims = new List<ApiScopeClaim>(),
                    },
                };

                List<ApiResource> apiResources = new List<ApiResource>()
                {
                    new ApiResource()
                    {
                        
                        Name = "api1",
                        DisplayName = "api1",
                        Scopes = new List<ApiResourceScope>()
                        {
                            new ApiResourceScope()
                            {
                                
                                Scope = apiScopes.Where(p=>p.DisplayName == "api1").FirstOrDefault()?.Name
                            }
                        },
                        UserClaims = new List<ApiResourceClaim>()
                        {
                            new ApiResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.Subject,
                            },
                            new ApiResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.Role,
                            },
                            new ApiResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.Name,
                            },
                            new ApiResourceClaim()
                            {
                                
                                Type = JwtClaimTypes.NickName,
                            },
                        }
                    }
                };

                List<Client> clients = new List<Client>()
                {
                    new Client()
                    {
                        ClientId = "2515961661554757",
                        ClientName = "2515961661554757",
                        ClientUri = "",
                        RequireClientSecret = false,
                        RequireConsent = true,
                        AllowRememberConsent = false,
                        AllowPlainTextPkce = false,
                        AllowAccessTokensViaBrowser = true,
                        BackChannelLogoutSessionRequired = true,
                        AllowOfflineAccess = true,
                        UpdateAccessTokenClaimsOnRefresh = false,
                        AccessTokenType = 0,
                        AllowedGrantTypes = new List<ClientGrantType>()
                        {
                            new ClientGrantType()
                            {
                                GrantType = "authorization_code",
                            }
                        },
                        RequirePkce = true,
                        ClientSecrets = new List<ClientSecret>()
                        {
                            new ClientSecret()
                            {
                                Value = "oYjppIizd29W4eodalgf+Vry0MfyLunBPVZeFmOelvU="
                            }
                        },
                        RedirectUris = new List<ClientRedirectUri>()
                        {
                            new ClientRedirectUri()
                            {
                                RedirectUri = "https://localhost:5001/signin-oidc"
                            }
                        },
                        FrontChannelLogoutUri = "https://localhost:5001/signout-oidc",
                        AllowedScopes = new List<ClientScope>()
                        {
                            new ClientScope()
                            {
                                Scope = JwtClaimTypes.Subject,
                            },
                            new ClientScope()
                            {
                                Scope = "openid",
                            },
                            new ClientScope()
                            {
                                Scope = JwtClaimTypes.Email,
                            },
                            new ClientScope()
                            {
                                Scope = JwtClaimTypes.PhoneNumber,
                            },
                            new ClientScope()
                            {
                                Scope = JwtClaimTypes.Profile,
                            },
                            new ClientScope()
                            {
                                Scope = "roles",
                            },
                            new ClientScope()
                            {
                                Scope = "api1",
                            },
                        }
                    }
                };

                if (!configurationDbContext.IdentityResources.Any())
                {
                    configurationDbContext.IdentityResources.AddRange(resources);
                }

                if (!configurationDbContext.ApiScopes.Any())
                {
                    configurationDbContext.ApiScopes.AddRange(apiScopes);
                }

                if (!configurationDbContext.ApiResources.Any())
                {
                    configurationDbContext.ApiResources.AddRange(apiResources);
                }

                if (!configurationDbContext.Clients.Any())
                {
                    configurationDbContext.Clients.AddRange(clients);
                }

                configurationDbContext.SaveChanges();
            }
            return app;
        }
    }
}
