using System;
using System.Collections.Generic;
using System.Reflection;
using NullPointer.AspNetCore.Entity.Builders;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class DbContextBuilderExtensions
    {
        public static DbContextBuilder WithRestModels(this DbContextBuilder contextBuilder, Assembly assembly)
        {
            IEnumerable<Type> assemblyRestModelTypes = assembly.GetRestModelTypes();
            
            foreach (Type restModelType in assemblyRestModelTypes)
                contextBuilder.WithEntity(restModelType);

            return contextBuilder;
        }

        public static DbContextBuilder WithRestModels(this DbContextBuilder contextBuilder)
        {
            return contextBuilder.WithRestModels(Assembly.GetExecutingAssembly());
        }
    }
}