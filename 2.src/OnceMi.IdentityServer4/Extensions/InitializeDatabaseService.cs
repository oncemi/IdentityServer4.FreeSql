using IdentityModel;
using IdentityServer4;
using IdentityServer4.FreeSql.Storage.Entities;
using IdentityServer4.FreeSql.Storage.Interfaces;
using IdentityServer4.FreeSql.User;
using IdentityServer4.FreeSql.User.Entities;
using IdentityServer4.FreeSql.User.Enums;
using IdentityServer4.FreeSql.User.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnceMi.AspNetCore.IdGenerator;
using OnceMi.IdentityServer4.Extensions.Utils;
using OnceMi.IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OnceMi.IdentityServer4.Extensions
{
    public static class InitializeDatabaseService
    {
        public static IApplicationBuilder UseInitializeDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                IIdGeneratorService idGenerator = scope.ServiceProvider.GetRequiredService<IIdGeneratorService>();
                IConfigurationDbContext configurationDbContext = scope.ServiceProvider.GetRequiredService<IConfigurationDbContext>();
                IPersistedGrantDbContext persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<IPersistedGrantDbContext>();
                UserDbContext userDbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

                logger.LogInformation("Start seed database...");

                #region user
                string initDataPath = Path.Combine(AppContext.BaseDirectory, "testdata.json");
                if (!File.Exists(initDataPath))
                {
                    logger.LogWarning("初始化数据不存在，跳过数据初始化。");
                }
                TestUserData testData = JsonUtil.DeserializeStringToObject<TestUserData>(File.ReadAllText(initDataPath));
                if (!userDbContext.Roles.Select.Any() && testData.Roles != null)
                {
                    logger.LogInformation("初始化角色信息...");

                    foreach(var item in testData.Roles)
                    {
                        item.CreatedTime = DateTime.Now;
                    }
                    userDbContext.Roles.AddRange(testData.Roles);
                }

                if (!userDbContext.Users.Select.Any() && testData.Users != null)
                {
                    logger.LogInformation("初始化用户信息...");

                    foreach (var item in testData.Users)
                    {
                        //用户角色
                        if (item.UserRoles != null && item.UserRoles.Count > 0)
                        {
                            foreach(var q in item.UserRoles)
                            {
                                q.CreatedTime = DateTime.Now;
                                userDbContext.UserRole.Add(q);
                            }
                        }
                        item.Password = item.Create(SHA256(item.Password));
                        userDbContext.Users.Add(item);
                    }
                }

                userDbContext.SaveChanges();

                #endregion

                #region client

                if(!configurationDbContext.IdentityResources.Select.Any() && testData.Resources != null)
                {
                    logger.LogInformation("初始化IdentityResources信息...");

                    foreach (var item in testData.Resources)
                    {
                        if (item.UserClaims != null && item.UserClaims.Count > 0)
                        {
                            foreach (var q in item.UserClaims)
                            {
                                configurationDbContext.IdentityResourceClaims.Add(q);
                            }
                        }
                        item.Created = DateTime.Now;
                        configurationDbContext.IdentityResources.Add(item);
                    }
                }

                if (!configurationDbContext.ApiScopes.Select.Any() && testData.ApiScopes != null)
                {
                    logger.LogInformation("初始化ApiScopes信息...");

                    foreach (var item in testData.ApiScopes)
                    {
                        if (item.UserClaims != null && item.UserClaims.Count > 0)
                        {
                            foreach (var q in item.UserClaims)
                            {
                                configurationDbContext.ApiScopeClaims.Add(q);
                            }
                        }
                        configurationDbContext.ApiScopes.Add(item);
                    }
                }

                if (!configurationDbContext.ApiResources.Select.Any() && testData.ApiResources != null)
                {
                    logger.LogInformation("初始化ApiResources信息...");

                    foreach (var item in testData.ApiResources)
                    {
                        if (item.Scopes != null && item.Scopes.Count > 0)
                        {
                            foreach (var q in item.Scopes)
                            {
                                configurationDbContext.ApiResourceScopes.Add(q);
                            }
                        }
                        if (item.UserClaims != null && item.UserClaims.Count > 0)
                        {
                            foreach (var q in item.UserClaims)
                            {
                                configurationDbContext.ApiResourceClaims.Add(q);
                            }
                        }
                        item.Created = DateTime.Now;
                        configurationDbContext.ApiResources.Add(item);
                    }
                }

                if (!configurationDbContext.Clients.Select.Any() && testData.Clients != null)
                {
                    logger.LogInformation("初始化客户端信息...");

                    foreach (var item in testData.Clients)
                    {
                        if (item.AllowedGrantTypes != null && item.AllowedGrantTypes.Count > 0)
                        {
                            foreach (var q in item.AllowedGrantTypes)
                            {
                                configurationDbContext.ClientGrantTypes.Add(q);
                            }
                        }
                        if (item.ClientSecrets != null && item.ClientSecrets.Count > 0)
                        {
                            foreach (var q in item.ClientSecrets)
                            {
                                configurationDbContext.ClientSecrets.Add(q);
                            }
                        }
                        if (item.RedirectUris != null && item.RedirectUris.Count > 0)
                        {
                            foreach (var q in item.RedirectUris)
                            {
                                configurationDbContext.ClientRedirectUris.Add(q);
                            }
                        }
                        if (item.AllowedScopes != null && item.AllowedScopes.Count > 0)
                        {
                            foreach (var q in item.AllowedScopes)
                            {
                                configurationDbContext.ClientScopes.Add(q);
                            }
                        }
                        item.Created = DateTime.Now;
                        configurationDbContext.Clients.Add(item);
                    }
                }

                configurationDbContext.SaveChanges();

                #endregion
            }
            return app;
        }

        private static string SHA256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = System.Security.Cryptography.SHA256.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString().ToLower();
        }
    }
}
