using System;
using System.Collections.Generic;
using System.Linq;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Rest;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    class ClassForRestRegistryTest : RestModel
    {
    }

    public class RestRegistryTest
    {
        [Fact]
        public void CheckIfExceptionThrownOnInvalidTypeRegister()
        {
            RestRegistry registry = new RestRegistry();
            RestRegistryEntry entry = new RestRegistryEntry(typeof(string), RestAllowedOperations.All);
            Assert.Throws<ArgumentException>(() =>
            {
                registry.Register(entry);
            });
        }

        [Fact]
        public void CheckIfTypeIsRegistered()
        {
            RestRegistry registry = new RestRegistry();
            Type testType = typeof(ClassForRestRegistryTest);
            RestAllowedOperations testTypeAllowedOperations = RestAllowedOperations.All;
            RestRegistryEntry testTypeEntry = new RestRegistryEntry(testType, testTypeAllowedOperations);
            registry.Register(testTypeEntry);
            RestRegistryEntry registeredEntry = registry.Entries
                .SingleOrDefault(e => e.Type == testType);
            Assert.NotNull(registeredEntry);
        }

        [Fact]
        public void CheckIfTypeIsRegisteredWithValidAllowedOperations()
        {
            RestRegistry registry = new RestRegistry();
            Type testType = typeof(ClassForRestRegistryTest);
            RestAllowedOperations testTypeAllowedOperations = RestAllowedOperations.All;
            RestRegistryEntry testTypeEntry = new RestRegistryEntry(testType, testTypeAllowedOperations);
            registry.Register(testTypeEntry);
            RestRegistryEntry registeredEntry = registry.Entries
                .Single(e => e.Type == testType);
            Assert.Equal(testTypeAllowedOperations, registeredEntry.AllowedOperations);
        }
    }
}