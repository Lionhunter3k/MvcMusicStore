using System;
using System.Collections.Generic;
namespace MvcMusicStore.Models
{
    public class CartItem : AbstractEntity<int>
    {
        public virtual int Count { get; set; }
        
        public virtual Album Album { get; set; }
    }
}