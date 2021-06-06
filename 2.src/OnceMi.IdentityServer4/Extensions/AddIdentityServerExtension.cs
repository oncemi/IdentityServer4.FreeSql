using FreeSql;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.FreeSql.Storage.DbContexts;
using IdentityServer4.FreeSql.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace OnceMi.IdentityServer4.Extensions
{
    public static class AddIdentityServerExtension
    {
        public static IServiceCollection AddIdentityServerWithFreeSql(this IServiceCollection services)
        {

            using (var provider = services.BuildServiceProvider())
            {
                IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
                IWebHostEnvironment env = provider.GetRequiredService<IWebHostEnvironment>();

                var connectionString = configuration.GetValue<string>("DbConnectionString:ConnectionString");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Can not find database connection string from config files.");
                }
                //get datatype
                DataType dbType = configuration.GetValue<DataType>("DbConnectionString:DbType");
                if(dbType == DataType.Sqlite)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        connectionString = connectionString.Replace("/", "\\");
                    else
                        connectionString = connectionString.Replace("\\", "/");
                    if (connectionString.Contains("{root}", StringComparison.OrdinalIgnoreCase))
                        connectionString = connectionString.Replace("{root}", AppContext.BaseDirectory);
                }

                var dbBuilder = new FreeSqlBuilder()
                    .UseConnectionString(dbType, connectionString)
                    .UseAutoSyncStructure(true)
                    .UseNoneCommandParameter(true)
                    .CreateDatabaseIfNotExists();

                var configurationDbContext = dbBuilder.Build<ConfigurationDbContext>();
                var persistedGrantDbContext = dbBuilder.Build<PersistedGrantDbContext>();
                var userDbContext = dbBuilder.Build<UserDbContext>();

                services.AddSingleton<IFreeSql<ConfigurationDbContext>>(configurationDbContext);
                services.AddSingleton<IFreeSql<PersistedGrantDbContext>>(persistedGrantDbContext);
                services.AddSingleton<IFreeSql<UserDbContext>>(userDbContext);
                services.AddScoped<UserDbContext>();

                var builder = services.AddIdentityServer()
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseFreeSql(orm: configurationDbContext);
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseFreeSql(orm: persistedGrantDbContext);

                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
                    })
                    .AddUserIdentity();

                if (env.IsDevelopment())
                {
                    builder.AddDeveloperSigningCredential();
                }
                else
                {
                    string certPath = Path.Combine(AppContext.BaseDirectory, configuration.GetValue<string>("Certificates:Path"));
                    string certPwd = configuration.GetValue<string>("Certificates:Password");
                    if (!File.Exists(certPath))
                    {
                        throw new Exception("IdentityServer证书路径错误。");
                    }
                    builder.AddConfigurationStoreCache();
                    builder.AddSigningCredential(new X509Certificate2(certPath, certPwd));
                }
            }
            return services;
        }

        public static IIdentityServerBuilder AddUserIdentity(this IIdentityServerBuilder builder)
        {
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.IsEssential = true;
                // we need to disable to allow iframe for authorize requests
                options.Cookie.SameSite = SameSiteMode.None;
            });

            builder.Services.ConfigureExternalCookie(options =>
            {
                options.Cookie.IsEssential = true;
                // https://github.com/IdentityServer/IdentityServer4/issues/2595
                options.Cookie.SameSite = SameSiteMode.None;
            });

            builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorRememberMeScheme, options =>
            {
                options.Cookie.IsEssential = true;
            });

            builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorUserIdScheme, options =>
            {
                options.Cookie.IsEssential = true;
            });

            builder.AddProfileService<DefaultProfileService>();
            builder.AddResourceOwnerValidator<DefaultResourceOwnerPasswordValidator>();

            return builder;
        }
    }
}
