using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NullPointer.AspNetCore.Rest.Attributes;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class AssemblyExtensions
    {
        public static Type[] GetRestModelTypes(this Assembly assembly)
        {
            Type[] assmeblyTypes = assembly.GetTypes();
            return assmeblyTypes.Where(t => t.IsRestModel())
                .ToArray();
        }
    }
}