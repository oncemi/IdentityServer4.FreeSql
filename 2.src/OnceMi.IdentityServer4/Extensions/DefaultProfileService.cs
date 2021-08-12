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
            var user = await _userDbContext.Users.Where(p => p.Id.ToString() == sub && p.Status == UserStatus.Enable && !p.IsDeleted).FirstAsync();
            if (user == null)
                return;
            //get all request
            List<string> requestClaims = RequestClaims(context)?.Distinct().ToList();
            if(requestClaims == null || requestClaims.Count == 0)
                return;
            List<Claim> claims = new List<Claim>();
            foreach (var item in requestClaims)
            {
                switch (item)
                {
                    case JwtClaimTypes.Id:
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
                            List<Roles> roles = await _userDbContext.Roles
                                .Where(p => !p.IsDeleted && p.IsEnabled && p.Users.AsSelect().Any(x => x.Id.ToString() == sub))
                                .ToListAsync();
                            if (roles != null && roles.Count > 0)
                            {
                                foreach (var role in roles)
                                {
                                    claims.Add(new Claim(item, role.Id.ToString()));
                                }
                            }
                        }
                        break;
                    case OrganizeJwtClaimType.Organize:
                        {
                            List<Organizes> organizes = await _userDbContext.Organizes
                                .Where(p => !p.IsDeleted && p.IsEnabled && p.Users.AsSelect().Any(x => x.Id.ToString() == sub))
                                .ToListAsync();
                            if (organizes != null && organizes.Count > 0)
                            {
                                foreach (var organize in organizes)
                                {
                                    claims.Add(new Claim(item, organize.Id.ToString()));
                                }
                            }
                        }
                        break;
                    case JwtClaimTypes.Email:
                        {
                            claims.Add(new Claim(item, user.Email));
                            claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLower()));
                        }
                        break;
                    case JwtClaimTypes.PhoneNumber:
                        {
                            claims.Add(new Claim(item, user.PhoneNumber));
                            claims.Add(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString().ToLower()));
                        }
                        break;
                    case JwtClaimTypes.Address:
                        {
                            claims.Add(new Claim(item, user.Address));
                        }
                        break;
                    case JwtClaimTypes.Picture:
                        {
                            claims.Add(new Claim(item, user.Avatar ?? ""));
                        }
                        break;
                    case JwtClaimTypes.Gender:
                        {
                            claims.Add(new Claim(item, user.Gender.ToString() ?? ""));
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
            _logger.LogDebug("验证用户是否有效：{caller}", context.Caller);

            var sub = context.Subject?.GetSubjectId();
            if (sub == null)
                throw new Exception("获取用户信息失败，用户Id为空");

            var user = await _userDbContext.Users
                .Where(p => p.Id.ToString() == sub && p.Status == UserStatus.Enable && !p.IsDeleted)
                .FirstAsync();
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
            if (_httpContext.HttpContext.Request.Path.Value?.ToLower().StartsWith("/connect/userinfo") == true)
            {
                var userClaims = context.Subject.Claims?
                    .GroupBy(p => p.Type)
                    .Select(q => q.Key).ToList();
                result.AddRange(userClaims);
            }
            if (context.RequestedResources.Resources.ApiResources == null
                || context.RequestedResources.Resources.ApiResources.Count == 0)
            {
                return result;
            }
            //从允许的认证资源中取出允许的UserClaims
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
            //在ApiResources中包含的UserClaims，且在允许的UserClaims中
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
            //在ApiScopes中包含的UserClaims，且在允许的UserClaims中
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
            //如果请求包含了Profile，则将Profile包含的ClaimType添加到result中
            if (allowClaims.Any(p => p == JwtClaimTypes.Profile
            || result.Any(p => p == JwtClaimTypes.Profile)))
            {
                if (!result.Contains(JwtClaimTypes.Picture))
                    result.Add(JwtClaimTypes.Picture);
                if (!result.Contains(JwtClaimTypes.Gender))
                    result.Add(JwtClaimTypes.Gender);
            }
            return result;
        }
    }
}
