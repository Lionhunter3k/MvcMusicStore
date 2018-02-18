using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.Models;
using NHibernate.Linq;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Filters;
using MvcMusicStore.Models.Enums;
using MvcMusicStore.Infrastructure.Persistence;

namespace MvcMusicStore.Controllers
{
    [UserWatcher(Order=5)]
    [CustomFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    public class StoreController : NHibernateBaseController<PrincipalAdapter<int, MvcMusicStore.Models.Enums.Roles>>
    {
        //
        // GET: /Store/

        public ActionResult Index()
        {
            var genres = NHibernateSession.Query<Genre>().ToList();

            return View(genres);
        }

        //
        // GET: /Store/Browse?genre=Disco

        public ActionResult Browse(string genre)
        {
            // Retrieve Genre and its Associated Albums from database
            var genreModel = NHibernateSession.Query<Genre>().FetchMany(p=>p.Albums).Single(p=>p.Name == genre);
            return View(genreModel);
        }

        //
        // GET: /Store/Details/5

        public ActionResult Details(int id)
        {
            var album = NHibernateSession.Get<Album>(id);

            return View(album);
        }

        //
        // GET: /Store/GenreMenu

        [ChildActionOnly]
        public ActionResult GenreMenu()
        {
            var genres = NHibernateSession.Query<Genre>().ToList();

            return PartialView(genres);
        }

    }
}