

using CoreMusicStore.Filters;
using CoreMusicStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;
using NHibernate;
using System.Linq;

namespace CoreMusicStore.Controllers
{
    [TypeFilter(typeof(UserWatcherFilter), Order = 5)]
    [TypeFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    public class HomeController : Controller
    {
        private readonly ISession _session;

        public HomeController(ISession session)
        {
            this._session = session;
        }
        //
        // GET: /Home/
        public ActionResult Index()
        {
            // Get most popular albums
            var albums = _session.Query<Album>().OrderByDescending(p => p.OrderDetails.Count())
                .Take(5)
                .ToList();

            return View(albums);
        }
    }
}