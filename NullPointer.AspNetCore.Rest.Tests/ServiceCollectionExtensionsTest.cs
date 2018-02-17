using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.Extensions.DependencyInjection;
using NullPointer.AspNetCore.Entity.Builders;
using NullPointer.AspNetCore.Entity.Extensions;
using NullPointer.AspNetCore.Rest.Extensions;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Repositories;
using NullPointer.AspNetCore.Rest.Services.Rest;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    class ClassForServiceCollectionExtensionsText : RestModel
    {
    }

    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void CheckIfDataRepositoryIsRegistered()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity<ClassForServiceCollectionExtensionsText>();
            IServiceCollection services = new ServiceCollection();
            services.AddRest(contextBuilder);
            Type iDataRepositoryType = typeof(IDataRepository<>)
                .MakeGenericType(typeof(ClassForServiceCollectionExtensionsText));
            ServiceDescriptor dataRepositoryDescriptor = services
                .SingleOrDefault(d => d.ServiceType == iDataRepositoryType);
            Assert.NotNull(dataRepositoryDescriptor);
        }

        [Fact]
        public void CheckIfDataRepositoryHasValidImplementation()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity<ClassForServiceCollectionExtensionsText>();
            IServiceCollection services = new ServiceCollection();
            services.AddRest(contextBuilder);
            Type dataRepositoryType = typeof(DataRepository<>)
                .MakeGenericType(typeof(ClassForServiceCollectionExtensionsText));
            ServiceDescriptor dataRepositoryDescriptor = services
                .SingleOrDefault(d => d.ImplementationType == dataRepositoryType);
            Assert.NotNull(dataRepositoryDescriptor);
        }

        [Fact]
        public void CheckIfRestRegistryIsRegistered()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity<ClassForServiceCollectionExtensionsText>();
            IServiceCollection services = new ServiceCollection();
            services.AddRest(contextBuilder);
            ServiceDescriptor restRegistryDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(IRestRegistry));
            Assert.NotNull(restRegistryDescriptor);
        }

        [Fact]
        public void CheckIfRestRegistryHasValidScope()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity<ClassForServiceCollectionExtensionsText>();
            IServiceCollection services = new ServiceCollection();
            services.AddRest(contextBuilder);
            ServiceDescriptor restRegistryDescriptor = services
                .Single(d => d.ServiceType == typeof(IRestRegistry));
            ServiceLifetime restRegistryLifetime = restRegistryDescriptor.Lifetime;
            Assert.Equal(ServiceLifetime.Singleton, restRegistryLifetime);
        }

        [Fact]
        public void CheckIfRestRouteCreatorIsRegistered()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity<ClassForServiceCollectionExtensionsText>();
            IServiceCollection services = new ServiceCollection();
            services.AddRest(contextBuilder);
            ServiceDescriptor routeCreatorDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(IRestRouteCreator));
            Assert.NotNull(routeCreatorDescriptor);
        }
    }
}