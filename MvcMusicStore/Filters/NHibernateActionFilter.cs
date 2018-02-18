using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Infrastructure;
using System.Web.Mvc;
using NHibernate;
using MvcMusicStore.Infrastructure.Persistence;

namespace MvcMusicStore.Filters
{
    public class NHibernateSession<T> :  AbstractFilter where T : ISessionWrapper
    {
        public NHibernateSession(T session)
        {
            this.Session = session;
        }

        private readonly T Session;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session.BeginTransaction();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Session.Transaction == null || !Session.Transaction.IsActive)
                throw new NullReferenceException("Transaction is null or not active");
            if (filterContext.Exception != null)
            {
                Session.Transaction.Rollback();
            }
            else
            {
                Session.Transaction.Commit();
            }
        }
    }

}