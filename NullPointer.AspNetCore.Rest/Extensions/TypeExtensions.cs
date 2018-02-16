using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NullPointer.AspNetCore.Rest.Attributes;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsRestModel(this Type type)
        {
            return !type.IsAbstract && type.GetCustomAttribute<RestIgnoreAttribute>() == null && typeof(RestModel).IsAssignableFrom(type);
        }

        public static Type[] GetDefinedDbSetTypes(this Type type)
        {
            PropertyInfo[] typeProperties = type.GetProperties();
            Type dbSetType = typeof(DbSet<>);
            return (from property in typeProperties
                    let propertyType = property.PropertyType
                    where propertyType.IsGenericType
                    let propertyTypeGenericDefinition = propertyType.GetGenericTypeDefinition()
                    where dbSetType.IsAssignableFrom(propertyTypeGenericDefinition)
                    select propertyType.GetGenericArguments().Single())
                .ToArray();
        }
    }
}