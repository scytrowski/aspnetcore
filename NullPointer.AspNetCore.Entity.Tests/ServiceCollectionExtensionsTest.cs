using Microsoft.Extensions.DependencyInjection;
using Moq;
using NullPointer.AspNetCore.Entity.Extensions;
using NullPointer.AspNetCore.Entity.Builders;
using NullPointer.AspNetCore.Entity.Services.Database;
using Xunit;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace NullPointer.AspNetCore.Entity.Tests
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void CheckIfExceptionThrownOnNullContextBuilder()
        {
            ServiceCollection services = new ServiceCollection();
            Assert.Throws<ArgumentNullException>(() => 
            {
                services.AddDbContext((DbContextBuilder)null);
            });
        }

        [Fact]
        public void CheckIfOptionsCreatorIsRegistered()
        {
            Mock<IDbContextOptionsCreator> optionsCreatorMock = new Mock<IDbContextOptionsCreator>();
            IDbContextOptionsCreator optionsCreator = optionsCreatorMock.Object;
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithOnConfiguring(optionsCreator);
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext(contextBuilder);
            ServiceDescriptor optionsCreatorDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsCreator));
            Assert.NotNull(optionsCreatorDescriptor);
        }

        [Fact]
        public void CheckIfModelCreatorIsRegistered()
        {
            Mock<IDbContextModelCreator> modelCreatorMock = new Mock<IDbContextModelCreator>();
            IDbContextModelCreator modelCreator = modelCreatorMock.Object;
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithOnModelCreating(modelCreator);
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext(contextBuilder);
            ServiceDescriptor modelCreatorDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(IDbContextModelCreator));
            Assert.NotNull(modelCreatorDescriptor);
        }

        [Fact]
        public void CheckIfDbContextIsRegistered()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder();
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext(contextBuilder);
            ServiceDescriptor dbContextDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(DbContext));
            Assert.NotNull(dbContextDescriptor);
        }
    }
}