using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreMusicStore.ViewComponents
{
    public class GenreMenuViewComponent : ViewComponent
    {
        private readonly ISession _session;

        public GenreMenuViewComponent(NHibernate.ISession session)
        {
            this._session = session;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await _session.Query<Genre>().ToListAsync();

            return View(genres);
        }
    }
}
