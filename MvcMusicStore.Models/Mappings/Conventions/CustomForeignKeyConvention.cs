using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using System.Reflection;

namespace MvcMusicStore.Models.Mappings.Conventions
{
    public class CustomForeignKeyConvention
  : ForeignKeyConvention
    {
        protected override string GetKeyName(FluentNHibernate.Member property, Type type)
        {
            if (property == null)
                return type.Name + "_FK";

            return property.Name + "_FK";
        }
    }
}
