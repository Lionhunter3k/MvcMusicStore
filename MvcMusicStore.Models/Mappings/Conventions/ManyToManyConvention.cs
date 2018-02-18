using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MvcMusicStore.Models.Mappings.Conventions
{
    public class ManyToManyConvention : IHasManyToManyConvention
    {
        public void Apply(IManyToManyCollectionInstance instance)
        {
            if (instance.OtherSide == null)
            {
                instance.Table(
                    string.Format(
                        "{0}To{1}",
                        instance.EntityType.Name+"S",
                        instance.ChildType.Name+"S"));
            }
            else
            {
                instance.Inverse();
            }
        }
    }
}
