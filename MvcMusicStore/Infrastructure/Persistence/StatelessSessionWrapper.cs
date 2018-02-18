using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using System.Data;

namespace MvcMusicStore.Infrastructure.Persistence
{
    public class StatelessSessionWrapper : ISessionWrapper
    {
        private readonly IStatelessSession Session;

        public StatelessSessionWrapper(IStatelessSession session)
        {
            this.Session = session;
        }

        #region ISessionWrapper Members

        public ITransaction BeginTransaction()
        {
            return Session.BeginTransaction();
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return Session.BeginTransaction(isolationLevel);
        }

        public ITransaction Transaction { get { return Session.Transaction; } }

        public bool IsConnected
        {
            get { return Session.IsConnected; }
        }

        public bool IsOpen
        {
            get { return Session.IsOpen; }
        }

        #endregion
    }
}