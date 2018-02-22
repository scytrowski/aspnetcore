using System;
using System.Collections.Generic;
using System.Reflection;

namespace NullPointer.AspNetCore.Rest.Tests.Extensions
{
    public static class TypeExtensions
    {
        // https://stackoverflow.com/a/10261848
        public static IDictionary<string, TConst> GetConstants<TConst>(this Type type)
        {
            Type constantType = typeof(TConst);
            FieldInfo[] staticFields = type.GetFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );
            Dictionary<string, TConst> constants = new Dictionary<string, TConst>();

            foreach (FieldInfo staticField in staticFields)
            {
                if (staticField.FieldType == constantType && staticField.IsLiteral && !staticField.IsInitOnly)
                    constants[staticField.Name] = (TConst)staticField.GetValue(null);
            }

            return constants;
        }
    }
}