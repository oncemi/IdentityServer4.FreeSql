using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnceMi.AspNetCore.IdGenerator;
using OnceMi.IdentityServer4.Extensions;
using OnceMi.IdentityServer4.Extensions.Utils;
using OnceMi.IdentityServer4.Filters;
using OnceMi.IdentityServer4.Models;
using OnceMi.IdentityServer4.User;
using OnceMi.IdentityServer4.User.Entities;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OnceMi.IdentityServer4.Controllers
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserDbContext _userDbContext;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IIdGeneratorService _idGenerator;
        private readonly IHttpContextAccessor _accessor;

        public AccountController(IIdentityServerInteractionService interaction
            , IClientStore clientStore
            , IAuthenticationSchemeProvider schemeProvider
            , IEventService events
            , UserDbContext userDbContext
            , IIdGeneratorService idGenerator
            , IHttpContextAccessor accessor)
        {
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
            _userDbContext = userDbContext ?? throw new ArgumentNullException(nameof(UserDbContext));
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
            _accessor = accessor;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        #region 登录

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);
            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { scheme = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        /// <summary>
        /// 登录请求
        /// </summary>
        /// <param name="model"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context == null)
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Json(new ResultObject<LoginResult>(0)
                    {
                        Data = new LoginResult()
                        {
                            IsRedirect = true,
                            RedirectUrl = "/",
                        }
                    });
                }
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);
                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    //return this.LoadingPage("Redirect", model.ReturnUrl);
                }
                return Json(new ResultObject<LoginResult>(0)
                {
                    Data = new LoginResult()
                    {
                        IsRedirect = true,
                        RedirectUrl = model.ReturnUrl,
                    }
                });
            }

            if (!ModelState.IsValid)
            {
                var values = ModelState.Values.Where(p => p.ValidationState == ModelValidationState.Invalid)
                    .Select(p => string.Join('|', p.Errors.Select(x => x.ErrorMessage).ToList()))
                    .ToList();
                return Json(new ResultObject<LoginResult>(-1, $"登录失败，错误：{string.Join('|', values)}。"));
            }
            var userInfo = await _userDbContext.Users.Where(p => (p.Id.ToString() == model.Username
                || p.UserName == model.Username
                || (p.Email == model.Username && p.EmailConfirmed)
                || (p.PhoneNumber == model.Username && p.PhoneNumberConfirmed))
                && p.Status == UserStatus.Enable
                && !p.IsDeleted)
                .FirstAsync();
            if (userInfo != null && userInfo.Authenticate(model.Password))
            {
                UserAgentParser parser = new UserAgentParser(HttpContext);
                //写登录日志
                await _userDbContext.LoginHistory.AddAsync(new LoginHistory()
                {
                    Id = _idGenerator.NewId(),
                    UserId = userInfo.Id,
                    IP = parser.GetRequestIpAddress(),
                    Browser = parser.GetBrowser(),
                    OS = parser.GetOS(),
                    Device = parser.GetDevice(),
                    UserAgent = Request.Headers["User-Agent"],
                    Type = LoginHistoryType.Login,
                    Status = true,
                    Message = "登录成功",
                });
                await _userDbContext.SaveChangesAsync();

                //var user = _users.FindByUsername(model.Username);
                await _events.RaiseAsync(new UserLoginSuccessEvent(userInfo.UserName, userInfo.Id.ToString(), userInfo.NickName, clientId: context?.Client.ClientId));

                AuthenticationProperties props = null;
                if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                    };
                };

                var isuser = new IdentityServerUser(userInfo.Id.ToString())
                {
                    DisplayName = userInfo.UserName,
                    AuthenticationTime = DateTime.Now,
                };
                await HttpContext.SignInAsync(isuser, props);
                if (context != null)
                {
                    //if (context.IsNativeClient())
                    //{
                    //    // The client is native, so this change in how to
                    //    // return the response is for better UX for the end user.
                    //    return this.LoadingPage("Redirect", model.ReturnUrl);
                    //}

                    //// we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    //return Redirect(model.ReturnUrl);

                    return Json(new ResultObject<LoginResult>(0)
                    {
                        Data = new LoginResult()
                        {
                            IsRedirect = true,
                            RedirectUrl = model.ReturnUrl,
                        }
                    });
                }

                // request for a local page
                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Json(new ResultObject<LoginResult>(0)
                    {
                        Data = new LoginResult()
                        {
                            IsRedirect = true,
                            RedirectUrl = model.ReturnUrl,
                        }
                    });
                }
                else if (string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Json(new ResultObject<LoginResult>(0)
                    {
                        Data = new LoginResult()
                        {
                            IsRedirect = true,
                            RedirectUrl = "/",
                        }
                    });
                }
                else
                {
                    // user might have clicked on a malicious link - should be logged
                    return Json(new ResultObject<LoginResult>(-1, "登录成功，但系统无法针对你所在的系统进行跳转，请检查传入的URL。"));
                }
            }
            else
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
                return Json(new ResultObject<LoginResult>(-1, AccountOptions.InvalidCredentialsErrorMessage));
            }
        }

        #endregion

        #region 登出

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await HttpContext.SignOutAsync();
                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
                // save login history
                if (!string.IsNullOrEmpty(User.GetSubjectId()) && long.TryParse(User.GetSubjectId(), out long userId))
                {
                    UserAgentParser parser = new UserAgentParser(HttpContext);
                    //写离线日志
                    await _userDbContext.LoginHistory.AddAsync(new LoginHistory()
                    {
                        Id = _idGenerator.NewId(),
                        UserId = userId,
                        IP = parser.GetRequestIpAddress(),
                        Browser = parser.GetBrowser(),
                        OS = parser.GetOS(),
                        Device = parser.GetDevice(),
                        UserAgent = Request.Headers["User-Agent"],
                        Type = LoginHistoryType.Logout,
                        Status = true,
                        Message = "退出成功",
                    });
                    await _userDbContext.SaveChangesAsync();
                }
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        #endregion

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Private

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId,
                ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt
            };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        #endregion

    }
}
