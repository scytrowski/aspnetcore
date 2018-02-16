using System;
using Microsoft.EntityFrameworkCore;
using NullPointer.AspNetCore.Entity.Services.Database;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Repositories;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    class ClassForDataRepositoryTest : RestModel
    {
        public string Name { get; set; }
    }

    class DbContextForDataRepositoryTest : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        public virtual DbSet<ClassForDataRepositoryTest> TestSet { get; set; }
    }

    public class DataRepositoryTest
    {
        [Fact]
        public void CheckIfGetResultIsValid()
        {
            ClassForDataRepositoryTest testModel = new ClassForDataRepositoryTest();
            DataRepository<ClassForDataRepositoryTest> dataRepository = CreateDataRepository();
            dataRepository.DbContext.Add(testModel);
            dataRepository.DbContext.SaveChanges();
            int testModelId = testModel.Id;
            ClassForDataRepositoryTest addedModel = dataRepository.Get(testModelId);
            Assert.NotNull(addedModel);
        }

        [Fact]
        public void CheckIfModelIsAdded()
        {
            ClassForDataRepositoryTest testModel = new ClassForDataRepositoryTest();
            DataRepository<ClassForDataRepositoryTest> dataRepository = CreateDataRepository();
            dataRepository.Add(testModel);
            int testModelId = testModel.Id;
            ClassForDataRepositoryTest addedModel = dataRepository.DbContext.Find(
                typeof(ClassForDataRepositoryTest), 
                new object[] { testModelId }
            ) as ClassForDataRepositoryTest;
            Assert.NotNull(addedModel);
        }

        [Fact]
        public void CheckIfModelIsUpdated()
        {
            ClassForDataRepositoryTest testModel = new ClassForDataRepositoryTest();
            DataRepository<ClassForDataRepositoryTest> dataRepository = CreateDataRepository();
            dataRepository.Add(testModel);
            int testModelId = testModel.Id;
            testModel.Name = "test name";
            dataRepository.Update(testModel);
            ClassForDataRepositoryTest updatedModel = dataRepository.DbContext.Find(
                typeof(ClassForDataRepositoryTest), 
                new object[] { testModelId }
            ) as ClassForDataRepositoryTest;
            Assert.Equal(testModel.Name, updatedModel.Name);
        }

        [Fact]
        public void CheckIfModelIsDeleted()
        {
            ClassForDataRepositoryTest testModel = new ClassForDataRepositoryTest();
            DataRepository<ClassForDataRepositoryTest> dataRepository = CreateDataRepository();
            dataRepository.Add(testModel);
            int testModelId = testModel.Id;
            dataRepository.Delete(testModel);
            ClassForDataRepositoryTest deletedModel = dataRepository.DbContext.Find(
                typeof(ClassForDataRepositoryTest), 
                new object[] { testModelId }
            ) as ClassForDataRepositoryTest;
            Assert.Null(deletedModel);
        }

        private DataRepository<ClassForDataRepositoryTest> CreateDataRepository()
        {
            DbContext dbContext = new DbContextForDataRepositoryTest();
            return new DataRepository<ClassForDataRepositoryTest>(dbContext);
        }
    }
}