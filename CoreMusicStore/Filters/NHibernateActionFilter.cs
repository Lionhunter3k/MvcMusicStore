using CoreMusicStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CoreMusicStore.Filters
{
    public class NHibernateSession<T> : IActionFilter where T : ISessionWrapper
    {
        public NHibernateSession(T session)
        {
            this.Session = session;
        }

        private readonly T Session;

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session.BeginTransaction();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
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