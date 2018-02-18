using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using Common.Logging;

namespace MvcMusicStore.Controllers
{
    public abstract class NHibernateBaseController<T> : Controller
    {
        public ISession NHibernateSession { get; set; }

        public ILog Logger { get; set; }

        public T TypedSession
        {
            get
            {
                var loggedInUser = (T)Session["User"];
                if (loggedInUser != null)
                {
                    return loggedInUser;
                }
                else
                    throw new NullReferenceException("the typed session is null");
            }
            set
            {
                Session["User"] = value;
            }
        }


    }
}