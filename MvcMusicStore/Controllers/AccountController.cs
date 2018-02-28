using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MvcMusicStore.ViewModels;
using NHibernate;
using NHibernate.Linq;
using MvcMusicStore.Models;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Controllers;
using MvcMusicStore.Filters;
using System.Security.Principal;
using MvcMusicStore.Infrastructure.Persistence;
using NHibernate.Util;

namespace Mvc3ToolsUpdateWeb_Default.Controllers
{
    [UserWatcher(Order = 5)]
    [CustomFilter(typeof(NHibernateSession<StatefulSessionWrapper>),Order=0)]
    public class AccountController : NHibernateBaseController<PrincipalAdapter<int,MvcMusicStore.Models.Enums.Roles>>
    {
        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model, string returnUrl)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var user = this.NHibernateSession.Query<RegisteredUser>().SingleOrDefault(p => p.Username == model.UserName && p.Password == model.Password);
                    if (user != null)
                    {
                        var anonymousUser = NHibernateSession.Get<AnonymousUser>(TypedSession.UserId);
                        anonymousUser.Items.Where(p=> user.Items.Add(p)).ForEach(p=> anonymousUser.Items.Remove(p));
                        NHibernateSession.Delete(anonymousUser);
                        SetCredentialsFromTo(anonymousUser, user);
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
            else
            {
                return new ViewResult { ViewName = "AlreadyLoggedIn" };
            }
        }

        private void SetCredentialsFromTo(AnonymousUser anonymousUser, RegisteredUser user)
        {
            var loggedInUser = new PrincipalAdapter<int,MvcMusicStore.Models.Enums.Roles>(user.Username,user.Id,new GenericIdentity(user.Username),user.Role);
            this.HttpContext.User = loggedInUser;
            var cookie = new HttpCookie("user");
            cookie.Values.Add("isAnonymous",false.ToString());
            cookie.Values.Add("userId",user.Id.ToString());
            this.Response.Cookies.Set(cookie);
            this.TypedSession = loggedInUser;
        }

        //
        // GET: /Account/LogOff
        [Authentication(RedirectToAction="LogOn")]
        public ActionResult LogOff()
        {
            var anonymousUser = new AnonymousUser { LatestAddress = Request.UserHostAddress };
            var currentUser = new PrincipalAdapter<int,MvcMusicStore.Models.Enums.Roles>(anonymousUser.LatestAddress,anonymousUser.Id,new AnonymousIdentity(anonymousUser.LatestAddress),anonymousUser.Role);
            NHibernateSession.Save(anonymousUser);
            this.HttpContext.User = currentUser;
            var cookie = new HttpCookie("user");
            cookie.Values.Add("isAnonymous", true.ToString());
            cookie.Values.Add("userId", anonymousUser.Id.ToString());
            this.TypedSession = currentUser;
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
        public ActionResult Register(RegisterViewModel model)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    // Attempt to register the user
                    if (!NHibernateSession.Query<RegisteredUser>().Any(p => p.Username == model.UserName || p.Email == model.Email))
                    {
                        var anonymousUser = NHibernateSession.Get<AnonymousUser>(TypedSession.UserId);
                        MvcMusicStore.Models.Enums.Roles role = model.UserName == "Admin" ? MvcMusicStore.Models.Enums.Roles.Admin : MvcMusicStore.Models.Enums.Roles.User;
                        var newRegisteredUser = new RegisteredUser { Username = model.UserName, Password = model.Password, Role = role, Email = model.Email, LatestAddress = anonymousUser.LatestAddress  };
                        NHibernateSession.Save(newRegisteredUser);
                        NHibernateSession.Delete(anonymousUser);
                        SetCredentialsFromTo(anonymousUser, newRegisteredUser);
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
            else
            {
                return new ViewResult { ViewName = "AlreadyLoggedIn" };
            }
        }

        //
        // GET: /Account/ChangePassword

        [Authentication(RedirectToAction="LogOn")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [Authentication(RedirectToAction = "LogOn")]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = NHibernateSession.Get<RegisteredUser>(TypedSession.UserId);
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
        [Authentication(RedirectToAction = "LogOn")]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }
}
