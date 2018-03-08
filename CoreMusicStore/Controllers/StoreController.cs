using CoreMusicStore.Filters;
using CoreMusicStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMusicStore.Controllers
{
    [TypeFilter(typeof(UserWatcherFilter), Order = 5)]
    [TypeFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    public class StoreController : Controller
    {
        private readonly ISession _session;

        public StoreController(ISession session)
        {
            this._session = session;
        }
        //
        // GET: /Store/

        public async Task<ActionResult> Index()
        {
            var genres = await _session.Query<Genre>().ToListAsync();

            return View(genres);
        }

        //
        // GET: /Store/Browse?genre=Disco

        public async Task<ActionResult> Browse(string genre)
        {
            // Retrieve Genre and its Associated Albums from database
            var genreModel = await _session.Query<Genre>().FetchMany(p=>p.Albums).SingleAsync(p=>p.Name == genre);
            return View(genreModel);
        }

        //
        // GET: /Store/Details/5

        public ActionResult Details(int id)
        {
            var album = _session.Get<Album>(id);

            return View(album);
        }
    }
}