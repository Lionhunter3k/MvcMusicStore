using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MvcMusicStore.Models.Tests
{
    public static class ReflectionHelper
    {
        public static object GetPrivateFieldValue(object theObject, string fieldName)
        {
            var type = theObject.GetType();
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(theObject);
        }
    }
}
