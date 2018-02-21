using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NullPointer.AspNetCore.Rest.Attributes;
using NullPointer.AspNetCore.Rest.Builders;
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

        public static RestAllowedOperations GetRestAllowedOperations(this Type type)
        {
            if (!type.IsRestModel())
                return RestAllowedOperations.None;

            RestModelOptionsBuilder options = new RestModelOptionsBuilder();

            if (type.GetCustomAttribute<RestDisableGetAllAttribute>() != null)
                options = options.WithDisabledGetAll();

            if (type.GetCustomAttribute<RestDisableGetAttribute>() != null)
                options = options.WithDisabledGet();

            if (type.GetCustomAttribute<RestDisableAddAttribute>() != null)
                options = options.WithDisabledAdd();

            if (type.GetCustomAttribute<RestDisableUpdateAttribute>() != null)
                options = options.WithDisabledUpdate();

            if (type.GetCustomAttribute<RestDisableDeleteAttribute>() != null)
                options = options.WithDisabledDelete();

            return options.AllowedOperations; 
        }
    }
}