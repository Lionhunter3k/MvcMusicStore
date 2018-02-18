using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace MvcMusicStore.Infrastructure
{
    public class AnonymousIdentity : GenericIdentity
    {
        public AnonymousIdentity(string name)
            : base(name) { }

        public override bool IsAuthenticated
        {
            get
            {
                return false;
            }
        }
    }

    public class PrincipalAdapter<Key,Roles> : IPrincipal
    {
        public PrincipalAdapter(string name,Key userId,IIdentity identity,params Roles[] roles)
        {
            Identity = identity;
            this._roles = roles;
            UserId = userId;
            UserLogonDateTime = DateTime.Now;
        }

        public DateTime UserLogonDateTime { get; private set; }

        public IIdentity Identity { get; private set; }

        public Key UserId { get; private set; }

        private Roles[] _roles;

        public virtual bool IsInRole(string role)
        {
            return _roles.Any(p=>p.ToString().Contains(role));
        }
    }
}