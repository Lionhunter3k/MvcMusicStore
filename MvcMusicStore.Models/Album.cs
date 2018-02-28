using System;
using System.Collections.Generic;

namespace MvcMusicStore.Models
{
    public class Album : AbstractEntity<int>
    {
        public Album()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }

        public virtual string Title { get; set; }//convention to map all nullable properties as non-nullable in DB

        public virtual decimal Price { get; set; }//convention to map non-nullable properties as non-nullable in DB

        public virtual string AlbumArtUrl { get; set; }

        public virtual Genre Genre { get; set; }//cascade-none

        public virtual Artist Artist { get; set; }//save-update cascade

        public virtual ISet<OrderDetail> OrderDetails { get; set; }//no cascade

    }
}