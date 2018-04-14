using System;
using System.Collections.Generic;
using System.Linq;
using MvcMusicStore.Models;
using Microsoft.AspNetCore.Mvc;
using CoreMusicStore.Filters;
using CoreMusicStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using NHibernate;
using System.Threading.Tasks;
using NHibernate.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using CoreMusicStore.ViewModels;

namespace MvcMusicStore.Controllers
{
    [Authorize(Roles = "Admin")]
    [TypeFilter(typeof(UserWatcherFilter), Order = 5)]
    [TypeFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    public class StoreManagerController : Controller
    {
        private readonly ISession _session;

        public StoreManagerController(ISession session)
        {
            this._session = session;
        }
        //
        // GET: /StoreManager/

        public async Task<ViewResult> Index()
        {
            var albums = await _session.Query<Album>().ToListAsync();
            return View(albums);
        }

        //
        // GET: /StoreManager/Details/5

        public async Task<ViewResult> Details(int id)
        {
            Album album = await _session.GetAsync<Album>(id);
            return View(album);
        }

        //
        // GET: /StoreManager/Create

        public async Task<ActionResult> Create()
        {
            ViewBag.Genres = new SelectList(await _session.Query<Genre>().Select(p=>new {  p.Id,  p.Name}).ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _session.Query<Artist>().Select(p => new {  p.Id,  p.Name }).ToListAsync(), "Id", "Name");
            return View();
        } 

        //
        // POST: /StoreManager/Create

        [HttpPost]
        public async Task<ActionResult> Create(AlbumViewModel album)
        {
            if (ModelState.IsValid)
            {
                var artist = await _session.GetAsync<Artist>(album.ArtistId);
                var genre = await _session.GetAsync<Genre>(album.GenreId);
                var newAlbum = new Album { Artist = artist, Genre = genre, AlbumArtUrl = album.AlbumArtUrl, Price = album.Price, Title = album.Title };
                genre.Albums.Add(newAlbum);
                return RedirectToAction("Index");  
            }

            ViewBag.Genres = new SelectList(await _session.Query<Genre>().Select(p => new {  p.Id,  p.Name }).ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _session.Query<Artist>().Select(p => new {  p.Id,  p.Name }).ToListAsync(), "Id", "Name");
            return View(album);
        }
        
        //
        // GET: /StoreManager/Edit/5
 
        public async Task<ActionResult> Edit(int id)
        {
            Album album = await _session.GetAsync<Album>(id);
            ViewBag.Genres = new SelectList(await _session.Query<Genre>().Select(p => new {  p.Id,  p.Name }).ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _session.Query<Artist>().Select(p => new {  p.Id,  p.Name }).ToListAsync(), "Id", "Name");
            return View(album);
        }

        //
        // POST: /StoreManager/Edit/5

        [HttpPost]
        public async Task<ActionResult> Edit(EditAlbumViewModel album)
        {
            if (ModelState.IsValid)
            {
                var albumToBeEdited = await _session.GetAsync<Album>(album.AlbumId);
                var genre = await _session.GetAsync<Genre>(album.GenreId);
                var artist = await _session.GetAsync<Artist>(album.ArtistId);
                albumToBeEdited.Genre.Albums.Remove(albumToBeEdited);
                albumToBeEdited.Genre = genre;
                albumToBeEdited.Artist = artist;
                return RedirectToAction("Index");
            }
            ViewBag.Genres = new SelectList(await _session.Query<Genre>().Select(p => new {  p.Id,  p.Name }).ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _session.Query<Artist>().Select(p => new {  p.Id,  p.Name }).ToListAsync(), "Id", "Name");
            return View(album);
        }

        //
        // GET: /StoreManager/Delete/5
 
        public async Task<ActionResult> Delete(int id)
        {
            Album album = await _session.GetAsync<Album>(id);
            return View(album);
        }

        //
        // POST: /StoreManager/Delete/5

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Album album = await _session.GetAsync<Album>(id);
            _session.Delete(album);
            return RedirectToAction("Index");
        }
    }
}