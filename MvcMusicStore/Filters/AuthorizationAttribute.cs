using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcMusicStore.Filters
{
    public class AuthorizationAttribute : ActionFilterAttribute
    {
        public AuthorizationAttribute(params string[] allowedRoles)
        {
            this._allowedRoles = allowedRoles;
        }

        private string[] _allowedRoles;

        public string ViewName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_allowedRoles.Any(role => filterContext.HttpContext.User.IsInRole(role)))
                filterContext.Result = new ViewResult { ViewName = ViewName };
        }
    }

}