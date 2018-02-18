using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using System.Data;

namespace MvcMusicStore.Infrastructure.Persistence
{
    public interface ISessionWrapper
    {
        ITransaction BeginTransaction();

        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        ITransaction Transaction { get; }

        bool IsConnected { get; }

        bool IsOpen { get; }
    }
}