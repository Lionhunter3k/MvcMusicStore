using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;
using MvcMusicStore.Filters;
using NHibernate;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Models.Enums;
using MvcMusicStore.Infrastructure.Persistence;

namespace MvcMusicStore.Controllers
{
    [CustomFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    [UserWatcher(Order=5)]
    public class ShoppingCartController : NHibernateBaseController<PrincipalAdapter<int, MvcMusicStore.Models.Enums.Roles>>
    {
        // GET: /ShoppingCart/

        public ActionResult Index()
        {
            var user = NHibernateSession.Get<AnonymousUser>(TypedSession.UserId);
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

        public ActionResult AddToCart(int id)
        {
            var user = NHibernateSession.Get<AnonymousUser>(TypedSession.UserId);
            // Retrieve the album from the database
            var addedAlbum = NHibernateSession.Get<Album>(id);
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
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var user = NHibernateSession.Get<AnonymousUser>(TypedSession.UserId);
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
                Message = Server.HtmlEncode(cartItem.Album.Title) + " has been removed from your shopping cart.",
                CartTotal = user.Items.Select(p=> new { price = p.Count * p.Album.Price}).Sum(p=>p.price),
                CartCount = user.Items.Sum(p=>p.Count),
                ItemCount = 10,
                DeleteId = id
            };

            return Json(results);
        }

        //
        // GET: /ShoppingCart/CartSummary

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var user = NHibernateSession.Get<AnonymousUser>(TypedSession.UserId);

            ViewBag.CartCount = user.Items.Sum(p => p.Count);

            return PartialView("CartSummary");
        }
    }
}