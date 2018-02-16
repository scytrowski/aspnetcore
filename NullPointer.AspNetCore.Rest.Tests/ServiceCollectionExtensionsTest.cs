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
    }
}