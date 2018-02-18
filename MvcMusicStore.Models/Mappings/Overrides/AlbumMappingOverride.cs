using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;

namespace MvcMusicStore.Models.Mappings.Overrides
{
    public class AlbumMappingOverride : IAutoMappingOverride<Album>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Album> mapping)
        {
            mapping.HasMany(e => e.ManyOrderDetails).AsSet().KeyColumn("AlbumId").Inverse().Cascade.SaveUpdate();
        }
    }
}
