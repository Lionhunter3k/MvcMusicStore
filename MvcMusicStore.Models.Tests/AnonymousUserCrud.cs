using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MvcMusicStore.Models.Enums;

namespace MvcMusicStore.Models.Tests
{
    [TestFixture]
    public class EmployeeCrud : CrudFixture<AnonymousUser, int>
    {
        protected override AnonymousUser BuildEntity()
        {
            return new AnonymousUser { LatestAddress = "localhost", Role = Roles.Admin };
        }

        protected override void ModifyEntity(AnonymousUser entity)
        {
            var artist = new Artist{ Name = "John Lennon"};
            var genre = new Genre{ Description = "blah", Name = "rock"};
            var album = new Album { AlbumArtUrl = "www.google.ro", Artist = artist, Genre = genre, Price = 90.0m, Title = "lel" };
            genre.Albums.Add(album);
            Session.Save(artist);
            Session.Save(genre);
            entity.LatestAddress = "192.168.71.20";
            entity.Items.Add(new CartItem{ Album = album});
        }

        protected override void AssertAreEqual(AnonymousUser expectedEntity, AnonymousUser actualEntity)
        {
            Assert.AreEqual(expectedEntity.Items, actualEntity.Items);
            Assert.AreEqual(expectedEntity.LatestAddress, actualEntity.LatestAddress);
            Assert.AreEqual(expectedEntity.Role, actualEntity.Role);
        }

        protected override void AssertValidId(AnonymousUser entity)
        {
            Assert.That(entity.Id > 0);
        }
    }
}
