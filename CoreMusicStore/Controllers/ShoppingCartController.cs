using CoreMusicStore.Filters;
using CoreMusicStore.Infrastructure.Persistence;
using CoreMusicStore.Services;
using CoreMusicStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;
using NHibernate;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.Controllers
{
    [TypeFilter(typeof(UserWatcherFilter), Order = 5)]
    [TypeFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    public class ShoppingCartController : Controller
    {
        private readonly ISession _session;
        private readonly IUserService _userService;

        public ShoppingCartController(ISession session, IUserService userService)
        {
            this._session = session;
            this._userService = userService;
        }

        // GET: /ShoppingCart/

        public async Task<ActionResult> Index()
        {
            var user = await _userService.GetCurrentAnonymousUserAsync();
            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = user.Items,
                CartTotal = user.Items.Count
            };

            // Return the view
            return View(viewModel);
        }

        //
        // GET: /Store/AddToCart/5

        public async Task<ActionResult> AddToCart(int id)
        {
            var user = await _userService.GetCurrentAnonymousUserAsync();
            // Retrieve the album from the database
            var addedAlbum = await _session.GetAsync<Album>(id);
            // Add it to the shopping cart
            if (user.Items.Select(p => p.Album).Contains(addedAlbum))
            {
                user.Items.Single(p => p.Album == addedAlbum).Count++;
            }
            else
            {
                user.Items.Add(new CartItem { Album = addedAlbum, Count = 1 });
            }
            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5

        [HttpPost]
        public async Task<ActionResult> RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var user = await _userService.GetCurrentAnonymousUserAsync();
            var cartItem = user.Items.Single(p => p.Id == id);
            // Remove from cart
            if (cartItem.Count == 1)
            {
                user.Items.Remove(cartItem);
            }
            else
                cartItem.Count--;
            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = cartItem.Album.Title + " has been removed from your shopping cart.",
                CartTotal = user.Items.Select(p=> new { price = p.Count * p.Album.Price}).Sum(p=>p.price),
                CartCount = user.Items.Sum(p=>p.Count),
                ItemCount = user.Items.Count,
                DeleteId = id
            };

            return Json(results);
        }
    }
}