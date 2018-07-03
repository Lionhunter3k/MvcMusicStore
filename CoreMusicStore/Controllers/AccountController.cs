using CoreMusicStore.Filters;
using CoreMusicStore.Infrastructure.Persistence;
using CoreMusicStore.Services;
using CoreMusicStore.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.Models.Enums;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Util;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreMusicStore.Controllers
{
    [TypeFilter(typeof(UserWatcherFilter), Order = 5)]
    [TypeFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order=0)]
    public class AccountController : Controller
    {
        private readonly ISession _session;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public AccountController(ISession session, IAuthenticationService authenticationService, IUserService userService)
        {
            this._session = session;
            this._authenticationService = authenticationService;
            this._userService = userService;
        }
        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOn(LogOnViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = this._session.Query<RegisteredUser>().SingleOrDefault(p => p.Username == model.UserName && p.Password == model.Password);
                if (user != null)
                {
                    await _userService.CopyCartItemsFromAnonymousUserAsync(user);
                    await SetCredentialsAsync(user);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                       && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("WrongUsernameOrPassword", "The user name or password provided is incorrect.");
                }
            }
            return View(model);
        }

        private async Task SetCredentialsAsync(AnonymousUser user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(claimsIdentity);
            await _authenticationService.SignInAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties { IsPersistent = true });
        }

        //
        // GET: /Account/LogOff
        [Authorize]
        public async Task<ActionResult> LogOff()
        {
            var anonymousUser = await _userService.GetOrCreateAnonymousUserAsync();
            await SetCredentialsAsync(anonymousUser);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                if (!await _session.Query<RegisteredUser>().AnyAsync(p => p.Username == model.UserName || p.Email == model.Email))
                {
                    Roles role = model.UserName == "Admin" ? Roles.Admin : Roles.User;
                    var newRegisteredUser = new RegisteredUser { Username = model.UserName, Password = model.Password, Role = role, Email = model.Email, LatestAddress = HttpContext.Connection.RemoteIpAddress.ToString() };
                    await _session.SaveAsync(newRegisteredUser);
                    await _userService.CopyCartItemsFromAnonymousUserAsync(newRegisteredUser);
                    await SetCredentialsAsync(newRegisteredUser);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "This username or email already exists");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetCurrentRegisteredUserAsync();
                if (user.Password == model.OldPassword)
                {
                    user.Password = model.NewPassword;
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("WrongCurrentPassword", "The current password is incorrect.");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        [Authorize]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }
}
