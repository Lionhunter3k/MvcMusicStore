using System;
using System.Linq;
using System.Text;
using MvcMusicStore.Models.Enums;
using Iesi.Collections.Generic;

namespace MvcMusicStore.Models
{
    public class AnonymousUser : AbstractEntity<int>
    {
        public AnonymousUser()
        {
            this.Items = new HashedSet<CartItem>();
        }

        public virtual ISet<CartItem> Items { get; set; }

        public virtual Roles Role { get; set; }
        
        public virtual string LatestAddress { get; set; }

    }


    public class RegisteredUser : AnonymousUser
    {
        public RegisteredUser()
        {
            this.Orders = new HashedSet<Order>();
        }

        public virtual string Username { get; set; }

        public virtual string Password { get; set; }

        public virtual string Question { get; set; }

        public virtual string Answer { get; set; }

        public virtual string Email { get; set; }

        public virtual ISet<Order> Orders { get; set; }
    }
}
