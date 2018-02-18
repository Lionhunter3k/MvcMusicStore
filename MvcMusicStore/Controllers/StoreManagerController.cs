using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Filters;
using NHibernate.Linq;
using MvcMusicStore.ViewModels;
using MvcMusicStore.Models.Enums;
using MvcMusicStore.Infrastructure.Persistence;

namespace MvcMusicStore.Controllers
{
    [Authorization("Admin",ViewName = "NotAllowedWithThisRole",Order=15)]
    [Authentication(RedirectToAction = "LogOn",FromController="AccountController",Order=10)]
    [UserWatcher(Order = 5)]
    [CustomFilter(typeof(NHibernateSession<StatefulSessionWrapper>), Order = 0)]
    public class StoreManagerController : NHibernateBaseController<PrincipalAdapter<int, MvcMusicStore.Models.Enums.Roles>>
    {
        //
        // GET: /StoreManager/

        public ViewResult Index()
        {
            var albums = NHibernateSession.Query<Album>().ToList();
            return View(albums);
        }

        //
        // GET: /StoreManager/Details/5

        public ViewResult Details(int id)
        {
            Album album = NHibernateSession.Get<Album>(id);
            return View(album);
        }

        //
        // GET: /StoreManager/Create

        public ActionResult Create()
        {
            ViewBag.GenreId = new SelectList(NHibernateSession.Query<Genre>().Select(p=>new { Id = p.Id, Name = p.Name}).ToList(), "Id", "Name");
            ViewBag.ArtistId = new SelectList(NHibernateSession.Query<Artist>().Select(p => new { Id = p.Id, Name = p.Name }).ToList(), "Id", "Name");
            return View();
        } 

        //
        // POST: /StoreManager/Create

        [HttpPost]
        public ActionResult Create(AlbumViewModel album)
        {
            if (ModelState.IsValid)
            {
                var artist = NHibernateSession.Get<Artist>(album.ArtistId);
                var genre = NHibernateSession.Get<Genre>(album.GenreId);
                var newAlbum = new Album { Artist = artist, Genre = genre, AlbumArtUrl = album.AlbumArtUrl, Price = album.Price, Title = album.Title };
                genre.Albums.Add(newAlbum);
                return RedirectToAction("Index");  
            }

            ViewBag.GenreId = new SelectList(NHibernateSession.Query<Genre>().Select(p => new { Id = p.Id, Name = p.Name }).ToList(), "Id", "Name");
            ViewBag.ArtistId = new SelectList(NHibernateSession.Query<Artist>().Select(p => new { Id = p.Id, Name = p.Name }).ToList(), "Id", "Name");
            return View(album);
        }
        
        //
        // GET: /StoreManager/Edit/5
 
        public ActionResult Edit(int id)
        {
            Album album = NHibernateSession.Get<Album>(id);
            ViewBag.GenreId = new SelectList(NHibernateSession.Query<Genre>().Select(p => new { Id = p.Id, Name = p.Name }).ToList(), "Id", "Name");
            ViewBag.ArtistId = new SelectList(NHibernateSession.Query<Artist>().Select(p => new { Id = p.Id, Name = p.Name }).ToList(), "Id", "Name");
            return View(album);
        }

        //
        // POST: /StoreManager/Edit/5

        [HttpPost]
        public ActionResult Edit(EditAlbumViewModel album)
        {
            if (ModelState.IsValid)
            {
                var albumToBeEdited = NHibernateSession.Get<Album>(album.AlbumId);
                var genre = NHibernateSession.Get<Genre>(album.GenreId);
                var artist = NHibernateSession.Get<Artist>(album.ArtistId);
                albumToBeEdited.Genre.Albums.Remove(albumToBeEdited);
                albumToBeEdited.Genre = genre;
                albumToBeEdited.Artist = artist;
                return RedirectToAction("Index");
            }
            ViewBag.GenreId = new SelectList(NHibernateSession.Query<Genre>().Select(p => new { Id = p.Id, Name = p.Name }).ToList(), "Id", "Name");
            ViewBag.ArtistId = new SelectList(NHibernateSession.Query<Artist>().Select(p => new { Id = p.Id, Name = p.Name }).ToList(), "Id", "Name");
            return View(album);
        }

        //
        // GET: /StoreManager/Delete/5
 
        public ActionResult Delete(int id)
        {
            Album album = NHibernateSession.Get<Album>(id);
            return View(album);
        }

        //
        // POST: /StoreManager/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = NHibernateSession.Get<Album>(id);
            NHibernateSession.Delete(album);
            return RedirectToAction("Index");
        }
    }
}