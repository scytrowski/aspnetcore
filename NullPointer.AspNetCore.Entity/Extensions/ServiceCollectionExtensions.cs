using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NullPointer.AspNetCore.Entity.Builders;
using NullPointer.AspNetCore.Entity.Services.Database;

namespace NullPointer.AspNetCore.Entity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Extensions method that enables registering DbContext based on provided builder
        /// </summary>
        /// <param name="services"></param>
        /// <param name="contextBuilder">DbContext subtype builder that cannot be null</param>
        public static void AddDbContext(this IServiceCollection services, DbContextBuilder contextBuilder)
        {
            if (contextBuilder == null)
                throw new ArgumentNullException("Context builder cannot be null");
            
            IDbContextOptionsCreator optionsCreator = contextBuilder.OptionsCreator ?? new DelegatedDbContextOptionsCreator(_ => {});
            IDbContextModelCreator modelCreator = contextBuilder.ModelCreator ?? new DelegatedDbContextModelCreator(_ => {});
            Type dbContextType = contextBuilder.Build();
            services.AddSingleton(typeof(IDbContextOptionsCreator), optionsCreator);
            services.AddSingleton(typeof(IDbContextModelCreator), modelCreator);
            services.AddScoped(typeof(DbContext), dbContextType);
        }
    }
}