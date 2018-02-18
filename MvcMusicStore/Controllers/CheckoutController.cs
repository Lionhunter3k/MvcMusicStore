using System;
using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.Filters;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.ViewModels;
using MvcMusicStore.Models.Enums;
using MvcMusicStore.Infrastructure.Persistence;

namespace MvcMusicStore.Controllers
{
    [Authentication(RedirectToAction = "LogOn",FromController="AccountController",Order=5)]
    [Authorization("User","Admin",ViewName="NotAllowedWithThisRole",Order=10)]
    [CustomFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    public class CheckoutController : NHibernateBaseController<PrincipalAdapter<int, MvcMusicStore.Models.Enums.Roles>>
    {
        const string PromoCode = "FREE";

        //
        // GET: /Checkout/AddressAndPayment

        public ActionResult AddressAndPayment()
        {
            return View();
        }

        //
        // POST: /Checkout/AddressAndPayment

        [HttpPost]
        public ActionResult AddressAndPayment(OrderViewModel order)
        {
            if (ModelState.IsValid)
            {
                var user = NHibernateSession.Get<RegisteredUser>(TypedSession.UserId);
                if (string.Equals(order.PromoCode, PromoCode,
                    StringComparison.OrdinalIgnoreCase) == false)
                {
                    return View(order);
                }
                var newOrder = new Order
                {
                    Username = TypedSession.Identity.Name,
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
                NHibernateSession.Save(newOrder);
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
