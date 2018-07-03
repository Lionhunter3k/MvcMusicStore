using CoreMusicStore.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using MvcMusicStore.Models;
using MvcMusicStore.Models.Enums;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreMusicStore.Filters
{
    public class UserWatcherFilter : IAsyncActionFilter
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        public UserWatcherFilter(IUserService userService, IAuthenticationService authenticationService)
        {
            this._userService = userService;
            this._authenticationService = authenticationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.HttpContext.User.Identity.IsAuthenticated)
            {
                var anonymousUser = await _userService.GetOrCreateAnonymousUserAsync();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, anonymousUser.Id.ToString()),
                    new Claim(ClaimTypes.Role, anonymousUser.Role.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var userPrincipal = new ClaimsPrincipal(claimsIdentity);
                await _authenticationService.SignInAsync(context.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties { IsPersistent = true });
            }
            await next();
        }
    }
}