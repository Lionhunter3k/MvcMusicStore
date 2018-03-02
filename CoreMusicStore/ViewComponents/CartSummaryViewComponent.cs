using CoreMusicStore.Services;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreMusicStore.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public CartSummaryViewComponent(IUserService userService)
        {
            this._userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userService.GetOrCreateAnonymousUserAsync();
            ViewBag.CartCount = user.Items.Sum(p => p.Count);
            return View();
        }
    }
}
