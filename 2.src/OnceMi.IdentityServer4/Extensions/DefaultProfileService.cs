using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Linq;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using OnceMi.IdentityServer4.User;
using OnceMi.IdentityServer4.User.Entities;

namespace OnceMi.IdentityServer4.Extensions
{
    public class DefaultProfileService : IProfileService
    {
        private readonly UserDbContext _userDbContext;
        private readonly ILogger<DefaultProfileService> _logger;
        private readonly IHttpContextAccessor _httpContext;

        public DefaultProfileService(UserDbContext userDbContext
            , ILogger<DefaultProfileService> logger
            , IHttpContextAccessor httpContext)
        {
            _userDbContext = userDbContext ?? throw new ArgumentNullException(nameof(UserDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<DefaultProfileService>));
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(IHttpContextAccessor));
        }

        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (string.IsNullOrEmpty(sub))
                return;
            var user = await _userDbContext.Users.Where(p => p.Id.ToString() == sub && p.Status == UserStatus.Enable).FirstAsync();
            if (user == null)
                return;
            List<Claim> claims = new List<Claim>();
            //get all request
            List<string> requestClaims = RequestClaims(context);

            foreach (var item in requestClaims)
            {
                switch (item)
                {
                    case JwtClaimTypes.Subject:
                        {
                            claims.Add(new Claim(item, user.Id.ToString()));
                        }
                        break;
                    case JwtClaimTypes.Name:
                        {
                            claims.Add(new Claim(item, user.UserName));
                        }
                        break;
                    case JwtClaimTypes.NickName:
                        {
                            claims.Add(new Claim(item, user.NickName));
                        }
                        break;
                    case JwtClaimTypes.Role:
                        {
                            List<Roles> roles = await _userDbContext.Roles.Where(p => p.UserRoles.AsSelect().Any(x => x.UserId.ToString() == sub)).ToListAsync();
                            if (roles != null && roles.Count > 0)
                            {
                                foreach (var role in roles)
                                {
                                    if (string.IsNullOrEmpty(role.Name)) continue;
                                    claims.Add(new Claim(item, role.Name));
                                }
                            }
                        }
                        break;
                    case JwtClaimTypes.Email:
                        {
                            claims.Add(new Claim(item, user.Email));
                            claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));
                        }
                        break;
                    case JwtClaimTypes.PhoneNumber:
                        {
                            claims.Add(new Claim(item, user.PhoneNumber));
                            claims.Add(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()));
                        }
                        break;
                    default:
                        {

                        }
                        break;
                }
            }

            //可以添加自定义用户信息
            //claims.Add(new Claim("custum", "testvalue"));

            context.AddRequestedClaims(claims);
        }

        /// <summary>
        /// 验证用户是否有效
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("用户是否有效：{caller}", context.Caller);

            var sub = context.Subject?.GetSubjectId();
            if (sub == null)
                throw new Exception("获取用户信息失败，用户Id为空");

            var user = await _userDbContext.Users.Where(p => p.Id.ToString() == sub && p.Status == UserStatus.Enable).FirstAsync();
            context.IsActive = user != null;
        }

        /// <summary>
        /// 获取授权访问的claims
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private List<string> RequestClaims(ProfileDataRequestContext context)
        {
            ICollection<ParsedScopeValue> requestScope = context.RequestedResources.ParsedScopes;
            List<string> allowClaims = new List<string>();
            List<string> result = new List<string>();

            //userinfo request
            if(_httpContext.HttpContext.Request.Path.Value?.ToLower().StartsWith("/connect/userinfo") == true)
            {
                var userClaims = context.Subject.Claims?
                    .GroupBy(p=>p.Type)
                    .Select(q=>q.Key).ToList();
                result.AddRange(userClaims);
            }
            if (context.RequestedResources.Resources.ApiResources == null
                || context.RequestedResources.Resources.ApiResources.Count == 0)
            {
                return result;
            }
            foreach (var pItem in context.RequestedResources.Resources.IdentityResources)
            {
                //判断资源是否在请求的资源中
                if (!requestScope.Any(p => p.ParsedName == pItem.Name))
                    continue;
                if (pItem.UserClaims == null || pItem.UserClaims.Count == 0)
                    continue;
                foreach (var item in pItem.UserClaims)
                {
                    if (allowClaims.Contains(item))
                        continue;
                    allowClaims.Add(item);
                }
            }
            foreach (var pItem in context.RequestedResources.Resources.ApiResources)
            {
                if (pItem.UserClaims == null || pItem.UserClaims.Count == 0)
                    continue;
                foreach (var item in pItem.UserClaims)
                {
                    if (!allowClaims.Contains(item))
                        continue;
                    if (result.Contains(item))
                        continue;
                    result.Add(item);
                }
            }
            foreach (var pItem in context.RequestedResources.Resources.ApiScopes)
            {
                if (pItem.UserClaims == null || pItem.UserClaims.Count == 0)
                    continue;
                foreach (var item in pItem.UserClaims)
                {
                    if (!allowClaims.Contains(item))
                        continue;
                    if (result.Contains(item))
                        continue;
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
