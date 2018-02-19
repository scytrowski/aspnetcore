using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NullPointer.AspNetCore.Entity.Builders;
using NullPointer.AspNetCore.Entity.Extensions;
using NullPointer.AspNetCore.Rest.Builders;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Repositories;
using NullPointer.AspNetCore.Rest.Services.Rest;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static RestOptionsBuilder AddRest(this IServiceCollection services, DbContextBuilder contextBuilder)
        {
            services.AddDbContext(contextBuilder);
            IEnumerable<Type> restModelTypes = contextBuilder.RegisteredEntities
                .Where(t => t.IsRestModel());
            return services.RegisterRestModelTypes(restModelTypes);
        }

        public static RestOptionsBuilder AddRest<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>();
            Type dbContextType = typeof(TDbContext);
            IEnumerable<Type> restModelTypes = dbContextType.GetDefinedDbSetTypes()
                .Where(t => t.IsRestModel());
            return services.RegisterRestModelTypes(restModelTypes);
        }

        private static RestOptionsBuilder RegisterRestModelTypes(this IServiceCollection services, IEnumerable<Type> modelTypes)
        {
            IRestRegistry restRegistry = new RestRegistry();
            Type iDataRepositoryType = typeof(IDataRepository<>);
            Type dataRepositoryType = typeof(DataRepository<>);

            foreach (Type modelType in modelTypes)
            {
                RestAllowedOperations modelAllowedOperations = modelType.GetRestAllowedOperations();
                RestRegistryEntry modelTypeEntry = new RestRegistryEntry(modelType, modelAllowedOperations);
                restRegistry.Register(modelTypeEntry);
                services.AddScoped(
                    iDataRepositoryType.MakeGenericType(modelType),
                    dataRepositoryType.MakeGenericType(modelType)
                );
            }

            services.AddSingleton(typeof(IRestRegistry), restRegistry);
            services.AddSingleton<IRestRouter, RestRouter>();
            services.AddRouting();
            RestOptionsBuilder options = new RestOptionsBuilder(restRegistry);
            return options;
        }
    }
}