using CoreMusicStore.Filters;
using CoreMusicStore.Infrastructure.Persistence;
using CoreMusicStore.Services;
using CoreMusicStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;
using NHibernate;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [TypeFilter(typeof(UserWatcherFilter), Order = 5)]
    [TypeFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    public class CheckoutController : Controller
    {
        const string PromoCode = "FREE";

        private readonly ISession _session;
        private readonly IUserService _userService;

        public CheckoutController(ISession session, IUserService userService)
        {
            this._session = session;
            this._userService = userService;
        }
        //
        // GET: /Checkout/AddressAndPayment

        public ActionResult AddressAndPayment()
        {
            return View(new OrderViewModel());
        }

        //
        // POST: /Checkout/AddressAndPayment

        [HttpPost]
        public async Task<ActionResult> AddressAndPayment(OrderViewModel order)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetCurrentRegisteredUserAsync();
                if (string.Equals(order.PromoCode, PromoCode,
                    StringComparison.OrdinalIgnoreCase) == false)
                {
                    return View(order);
                }
                var newOrder = new Order
                {
                    Username = user.Username,
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    Country = order.Country,
                    Phone = order.Phone,
                    Address = order.Address,
                    City = order.City,
                    PostalCode = order.PostalCode,
                    State = order.State,
                    Email = user.Email,
                    OrderDate = DateTime.Now,
                    Total = user.Items.Select(p => new { total = p.Count * p.Album.Price }).Sum(p => p.total)
                };
                user.Items.ToList().ForEach(p => {
                    var newOrderDetails = new OrderDetail { Album = p.Album, Order = newOrder, Quantity = p.Count, UnitPrice = p.Album.Price };
                    newOrder.OrderDetails.Add(newOrderDetails);
                    p.Album.OrderDetails.Add(newOrderDetails);
                });
                _session.Save(newOrder);
                user.Items.Clear();
                return RedirectToAction("Complete",
              new { id = newOrder.Id });
            }
            else
            {
                   //Invalid - redisplay with errors
                return View(order);
            }
        }

        //
        // GET: /Checkout/Complete

        public ActionResult Complete(int id)
        {
            // Validate customer owns this order

            return View(id);
        }
    }
}
