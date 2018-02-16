using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NullPointer.AspNetCore.Entity.Builders;
using NullPointer.AspNetCore.Entity.Extensions;
using NullPointer.AspNetCore.Rest.Services.Repositories;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRest(this IServiceCollection services, DbContextBuilder contextBuilder)
        {
            services.AddDbContext(contextBuilder);
            Type iDataRepositoryType = typeof(IDataRepository<>);
            Type dataRepositoryType = typeof(DataRepository<>);

            foreach (Type entityType in contextBuilder.RegisteredEntities)
            {
                if (entityType.IsRestModel()) 
                {
                    services.AddScoped(
                        iDataRepositoryType.MakeGenericType(entityType),
                        dataRepositoryType.MakeGenericType(entityType)
                    );
                }
            }
        }

        public static void AddRest<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>();
            Type dbContextType = typeof(TDbContext);
            Type[] entityTypes = dbContextType.GetDefinedDbSetTypes();
            Type iDataRepositoryType = typeof(IDataRepository<>);
            Type dataRepositoryType = typeof(DataRepository<>);

            foreach (Type entityType in entityTypes)
            {
                if (entityType.IsRestModel())
                {
                    services.AddScoped(
                        iDataRepositoryType.MakeGenericType(entityType),
                        dataRepositoryType.MakeGenericType(entityType)
                    );
                }
            }
        }
    }
}