using System;
using System.Collections.Generic;
using NullPointer.AspNetCore.Rest.Services.Rest;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class RestRegisterExtensions
    {
        public static void RegisterAll(this IRestRegistry registry, IEnumerable<Type> modelTypes)
        {
            foreach (Type modelType in modelTypes)
                registry.Register(modelType);
        }
    }
}