using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.Filters;
using NHibernate;
using NHibernate.Linq;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Models.Enums;
using MvcMusicStore.Infrastructure.Persistence;

namespace MvcMusicStore.Controllers
{
    [CustomFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    [UserWatcher(Order=5)]
    public class HomeController : NHibernateBaseController<PrincipalAdapter<int,MvcMusicStore.Models.Enums.Roles>>
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            // Get most popular albums
            var albums = NHibernateSession.Query<Album>().OrderByDescending(p => p.OrderDetails.Count())
                .Take(5)
                .ToList();

            return View(albums);
        }
    }
}