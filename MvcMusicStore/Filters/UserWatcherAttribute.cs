using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Models;
using System.Security.Principal;
using MvcMusicStore.Models.Enums;

namespace MvcMusicStore.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class UserWatcherAttribute : ActionFilterAttribute
    {
        public ISession Session { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var currentUser = (PrincipalAdapter<int, MvcMusicStore.Models.Enums.Roles>)filterContext.HttpContext.Session["User"];
            if (currentUser == null)
            {
                if (!filterContext.HttpContext.Request.Cookies.AllKeys.Contains("user"))
                {
                    var anonymousUser = new AnonymousUser { Role = Roles.Anonymous, LatestAddress = filterContext.HttpContext.Request.UserHostAddress };
                    Session.Save(anonymousUser);
                    var newCookie = new HttpCookie("user");
                    newCookie.Values.Set("isAnonymous", "true");
                    newCookie.Values.Set("userId", anonymousUser.Id.ToString());
                    newCookie.HttpOnly = true;
                    newCookie.Expires = DateTime.Today.AddDays(2.0);
                    currentUser = new PrincipalAdapter<int, MvcMusicStore.Models.Enums.Roles>(anonymousUser.LatestAddress, anonymousUser.Id, new AnonymousIdentity(anonymousUser.LatestAddress), anonymousUser.Role);
                    filterContext.HttpContext.Response.AppendCookie(newCookie);
                    filterContext.HttpContext.Session["User"] = currentUser;
                    filterContext.HttpContext.User = currentUser;
                }
                else
                {
                    var cookie = filterContext.HttpContext.Request.Cookies.Get("user");
                    bool isAnonymous = Convert.ToBoolean(cookie["isAnonymous"]);
                    int userId = Convert.ToInt32(cookie["userId"]);
                    if (isAnonymous)
                    {
                        var anonymousUser = Session.Get<AnonymousUser>(userId);
                        if (anonymousUser != null)
                        {
                            currentUser = new PrincipalAdapter<int, MvcMusicStore.Models.Enums.Roles>(anonymousUser.LatestAddress, anonymousUser.Id, new AnonymousIdentity(anonymousUser.LatestAddress), anonymousUser.Role);
                            filterContext.HttpContext.Session["User"] = currentUser;
                            filterContext.HttpContext.User = currentUser;
                        }
                        else
                        {
                            anonymousUser = new AnonymousUser { Role = Roles.Anonymous, LatestAddress = filterContext.HttpContext.Request.UserHostAddress };
                            Session.Save(anonymousUser);
                            var newCookie = new HttpCookie("user");
                            newCookie.Values.Set("isAnonymous", "true");
                            newCookie.Values.Set("userId", anonymousUser.Id.ToString());
                            newCookie.HttpOnly = true;
                            newCookie.Expires = DateTime.Today.AddDays(2.0);
                            filterContext.HttpContext.Response.AppendCookie(newCookie);
                            currentUser = new PrincipalAdapter<int, MvcMusicStore.Models.Enums.Roles>(anonymousUser.LatestAddress, anonymousUser.Id, new AnonymousIdentity(anonymousUser.LatestAddress), anonymousUser.Role);
                            filterContext.HttpContext.Session["User"] = currentUser;
                            filterContext.HttpContext.User = currentUser;
                        }
                    }
                    else
                    {
                        var loggedInUser = Session.Get<RegisteredUser>(userId);
                        currentUser = new PrincipalAdapter<int, MvcMusicStore.Models.Enums.Roles>(loggedInUser.Username, loggedInUser.Id, new GenericIdentity(loggedInUser.Username), loggedInUser.Role);
                        filterContext.HttpContext.Session["User"] = currentUser;
                    }
                }
            }
            else
            {
                filterContext.HttpContext.User = currentUser;
            }
        }
    }
}