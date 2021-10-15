using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using OnceMi.IdentityServer4.User;

namespace OnceMi.IdentityServer4.Extensions
{
    public class DefaultResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserDbContext _userDbContext;
        private readonly ILogger<DefaultResourceOwnerPasswordValidator> _logger;

        public DefaultResourceOwnerPasswordValidator(UserDbContext userDbContext
            , ILogger<DefaultResourceOwnerPasswordValidator> logger)
        {
            _userDbContext = userDbContext ?? throw new ArgumentNullException(nameof(UserDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<DefaultResourceOwnerPasswordValidator>));
        }

        /// <summary>
        /// 验证登录信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var userInfo = await _userDbContext.Users.Where(p => p.UserName == context.UserName
                || (p.Email == context.UserName && p.EmailConfirmed)
                || (p.PhoneNumber == context.UserName && p.PhoneNumberConfirmed)).FirstAsync();
            if (!userInfo.AuthenticatePassword(context.Password))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户名或密码错误！");
                return;
            }
            context.Result = new GrantValidationResult(userInfo.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
        }
    }
}
