using System;
using Iesi.Collections.Generic;

namespace MvcMusicStore.Models
{
    public class Order : AbstractEntity<int>
    {
        public Order()
        {
            this.OrderDetails = new HashedSet<OrderDetail>();
        }

        public virtual DateTime OrderDate { get; set; }

        public virtual string Username { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Address { get; set; }

        public virtual string City { get; set; }

        public virtual string State { get; set; }

        public virtual string PostalCode { get; set; }

        public virtual string Country { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Email { get; set; }

        public virtual decimal Total { get; set; }

        public virtual ISet<OrderDetail> OrderDetails { get; set; }
    }
}
