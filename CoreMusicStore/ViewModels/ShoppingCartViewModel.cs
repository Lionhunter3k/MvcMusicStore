using System.Collections.Generic;
using MvcMusicStore.Models;

namespace CoreMusicStore.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<CartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}