using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;

namespace MvcMusicStore.Models.Mappings.Conventions
{
    public class DefaultStringLengthConvention
  : IPropertyConvention, IPropertyConventionAcceptance
    {

        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {

            criteria.Expect(x => x.Type == typeof(string))

            .Expect(x => x.Length == 0);

        }

        public void Apply(IPropertyInstance instance)
        {

            instance.Length(4000);

        }

    }
}

