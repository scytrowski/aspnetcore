using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NullPointer.AspNetCore.Entity.Builders;
using NullPointer.AspNetCore.Entity.Services.Database;
using Xunit;

namespace NullPointer.AspNetCore.Entity.Tests
{
    public class ClassForDbContextBuilderTest
    {
    }

    public class DbContextBuilderTest
    {
        [Fact]
        public void CheckIfExceptionThrownOnNullEntityType()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder();
            Assert.Throws<ArgumentNullException>(() => 
            {
                contextBuilder.WithEntity(null);
            });
        }

        [Fact]
        public void CheckIfExceptionThrownOnNonClassEntityType()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder();
            Assert.Throws<ArgumentException>(() => 
            {
                contextBuilder.WithEntity(typeof(int));
            });
        }

        [Fact]
        public void CheckIfEntityTypeIncluded()
        {
            Type includedEntityType = typeof(object);
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity(includedEntityType);
            Assert.Contains(includedEntityType, contextBuilder.RegisteredEntities);
        }

        [Fact]
        public void CheckIfExceptionThrownOnNullOptionsCreator()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder();
            Assert.Throws<ArgumentNullException>(() => 
            {
                contextBuilder.WithOnConfiguring((IDbContextOptionsCreator)null);
            });
        }

        [Fact]
        public void CheckIfExceptionThrownOnNullModelCreator()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder();
            Assert.Throws<ArgumentNullException>(() => 
            {
                contextBuilder.WithOnModelCreating((IDbContextModelCreator)null);
            });
        }

        [Fact]
        public void CheckIfBuiltTypeIsClass()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder();
            Type dbContextType = contextBuilder.Build();
            Assert.True(dbContextType.IsClass);
        }

        [Fact]
        public void CheckIfBuiltTypeHasValidParent()
        {
            DbContextBuilder contextBuilder = new DbContextBuilder();
            Type dbContextType = contextBuilder.Build();
            Assert.Equal(typeof(ConfigurableDbContext), dbContextType.BaseType);
        }

        [Fact]
        public void CheckIfBuiltTypeHasValidDbSet()
        {
            Type testClassType = typeof(ClassForDbContextBuilderTest);
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity(testClassType);
            Type dbContextType = contextBuilder.Build();
            Type expectedDbSetType = typeof(DbSet<>)
                .MakeGenericType(testClassType);
            PropertyInfo dbSetProperty = dbContextType.GetProperties()
                .SingleOrDefault(p => p.PropertyType == expectedDbSetType);
            Assert.NotNull(dbSetProperty);
        }
    }
}