using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using System.Reflection;

namespace MvcMusicStore.Models.Mappings.Conventions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllDeleteOrphansAttribute : Attribute
    {
    }

    public abstract class AutoMapperConvention<T> where T : Attribute
    {
        protected bool HasAttribute(ICustomAttributeProvider provider)
        {
            return provider.GetCustomAttributes(typeof(T), false).Length == 1;
        }
    }


    class AllDeleteOrphansOneToManyCascadeConvention :AutoMapperConvention<AllDeleteOrphansAttribute>, IHasManyConvention, IHasManyConventionAcceptance
    {
        public void Accept(FluentNHibernate.Conventions.AcceptanceCriteria.IAcceptanceCriteria<FluentNHibernate.Conventions.Inspections.IOneToManyCollectionInspector> criteria)
        {
            criteria.Expect(p => this.HasAttribute(p.Member));
        }

        public void Apply(FluentNHibernate.Conventions.Instances.IOneToManyCollectionInstance instance)
        {
            instance.Inverse();
            instance.Cascade.SaveUpdate();
        }

    }
}
