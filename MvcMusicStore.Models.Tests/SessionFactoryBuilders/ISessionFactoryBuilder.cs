using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace MvcMusicStore.Models.Tests.SessionFactoryBuilders
{
    interface ISessionFactoryBuilder
    {
        ISessionFactory BuildSessionFactory();
    }
}
