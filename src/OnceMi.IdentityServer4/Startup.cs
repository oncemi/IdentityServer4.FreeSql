using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnceMi.AspNetCore.IdGenerator;
using OnceMi.IdentityServer4.Extensions;
using OnceMi.IdentityServer4.Extensions.Utils;
using System;
using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace OnceMi.IdentityServer4
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public readonly IConfiguration Configuration;

        private const string _defaultOrigins = "DefaultCorsPolicy";

        public Startup(IWebHostEnvironment env
            , IConfiguration configuration)
        {
            Configuration = configuration;
            _env = env ?? throw new ArgumentNullException(nameof(IWebHostEnvironment));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(120);
            });

            #region Cors 跨域
            services.AddCors(options =>
            {
                options.AddPolicy(_defaultOrigins, policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            #endregion

            #region 雪花算法

            services.AddIdGenerator(x =>
            {
                x.AppId = Configuration.GetValue<ushort>("AppSettings:AppId");
            });

            #endregion

            services.AddIdentityServerWithFreeSql();

            services.AddHttpContextAccessor();

            services.AddControllersWithViews()
                .AddJsonOptions(options =>
            {
                //中文等特殊字符序列化
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                //包含公共字段
                options.JsonSerializerOptions.IncludeFields = true;
                //忽略大小写
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                //DateTime
                options.JsonSerializerOptions.Converters.Add(new Extensions.Utils.DateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new Extensions.Utils.DateTimeNullableConverter());
                //小驼峰
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors(_defaultOrigins);
            app.UseRouting();
            app.UseSession();

            app.UseSqlLogger();
            app.UseInitializeDatabase();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
