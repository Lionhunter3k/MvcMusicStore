using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using System.Reflection;

namespace MvcMusicStore.Models.Mappings.Conventions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SaveUpdateAttribute : Attribute
    {
    }

    public class SaveUpdateOneToManyCascadeConvention :AutoMapperConvention<SaveUpdateAttribute>, IReferenceConvention,IReferenceConventionAcceptance,IHasManyConvention, IHasManyConventionAcceptance
    {
        public void Apply(IOneToManyCollectionInstance instance)
        {
            if (instance.OtherSide != null)
                instance.Inverse();
            else
            {
                instance.Not.KeyNullable();
                instance.
            }
            instance.Cascade.SaveUpdate();
        }

        public void Accept(IAcceptanceCriteria<IOneToManyCollectionInspector> criteria)
        {
            criteria.Expect(p => this.HasAttribute(p.Member));
        }


        public void Accept(IAcceptanceCriteria<IManyToOneInspector> criteria)
        {
            criteria.Expect(p => this.HasAttribute(p.Property.MemberInfo));
        }

        public void Apply(IManyToOneInstance instance)
        {
            instance.Not.Nullable();
            instance.Cascade.SaveUpdate();
        }

    }
}
