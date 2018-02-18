﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using System.Data;

namespace MvcMusicStore.Infrastructure.Persistence
{
    public class StatefulSessionWrapper : ISessionWrapper
    {
        public StatefulSessionWrapper(ISession session)
        {
            this.Session = session;
        }

        private readonly ISession Session;

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