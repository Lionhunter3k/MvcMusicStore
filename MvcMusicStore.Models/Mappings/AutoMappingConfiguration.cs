using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate;

namespace MvcMusicStore.Models.Mappings
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(System.Type type)
        {
            // there's a bug in FluentNHibernate 1.3 where it automatically tries to map compiler generated classes
            return type.Namespace == "MvcMusicStore.Models" && type.Name.IndexOf("DisplayClass") < 0;
        }
        
        
        public override bool IsComponent(System.Type type)
        {
            return type.Namespace == "MvcMusicStore.Models.Components";
        }

        public override bool IsId(Member member)
        {
            return member.Name == "Id";
        }

        public override string GetComponentColumnPrefix(Member member)
        {
            return member.PropertyType.Name + "_";
        }

        public override bool IsDiscriminated(Type type)
        {
            return false;
        }
    }
}
