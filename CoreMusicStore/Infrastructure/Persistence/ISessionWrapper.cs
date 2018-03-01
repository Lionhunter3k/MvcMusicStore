using NHibernate;
using System.Data;

namespace CoreMusicStore.Infrastructure.Persistence
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