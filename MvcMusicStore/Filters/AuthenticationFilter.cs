using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Logging;
using System.Web.Routing;
using NHibernate;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Models;

namespace MvcMusicStore.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthenticationAttribute : ActionFilterAttribute
    {
        public string RenderView
        {
            get
            {
                return _viewName;
            }
            set
            {
                _shouldRenderView = true;
                _viewName = value;
            }
        }

        public string RedirectToAction
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
            }
        }

        public string FromController
        {
            get
            {
                return _controller;
            }
            set
            {
                _controller = value.Replace("Controller", string.Empty);
                this._redirectTargetDictionary = new RouteValueDictionary(new { controller = _controller, action = this._action });
            }
        }

        public ILog Logger { get; set; }

        private string _action;

        private string _controller;

        private string _viewName;

        private bool _shouldRenderView = false;

        private RouteValueDictionary _redirectTargetDictionary;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
                Logger.Debug(m => m("The action <{0}> from controller <{1}> is requesting authentification", filterContext.ActionDescriptor.ActionName, filterContext.Controller.GetType()));
                if (null == filterContext.HttpContext.User || !filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    Logger.Warn(m => m("There is no authentificated logged in user for the executing action <{0}> from controller <{1}>. The current request came from <{2}>.", filterContext.ActionDescriptor.ActionName, filterContext.Controller.GetType(), filterContext.HttpContext.Request.UserHostAddress));
                    if (_shouldRenderView)
                    {
                        filterContext.Result = new ViewResult { ViewName = this.RenderView };
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(_controller))
                        {
                            _redirectTargetDictionary = new RouteValueDictionary(new { controller = filterContext.Controller.GetType().Name.Replace("Controller",string.Empty), action = _action });
                        } 
                        filterContext.Result = new RedirectToRouteResult(_redirectTargetDictionary);
                    }
                }
                else
                    Logger.Debug(m => m("The currently logged in user is :\n <{0}> .", filterContext.HttpContext.User.Identity.Name));
        }

    }
}