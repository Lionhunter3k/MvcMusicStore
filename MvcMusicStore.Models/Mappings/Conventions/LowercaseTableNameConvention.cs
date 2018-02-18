using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MvcMusicStore.Models.Mappings.Conventions
{
    public class PrefixedTableNameConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table("tbl_" + instance.EntityType.Name);
        }
    }
}
