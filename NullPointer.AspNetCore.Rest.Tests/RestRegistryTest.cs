using System;
using System.Collections.Generic;
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
            Assert.Throws<ArgumentException>(() =>
            {
                registry.Register(typeof(string));
            });
        }

        [Fact]
        public void CheckIfTypeIsRegistered()
        {
            RestRegistry registry = new RestRegistry();
            Type testType = typeof(ClassForRestRegistryTest);
            registry.Register(testType);
            IEnumerable<Type> registeredTypes = registry.RegisteredModelTypes;
            Assert.Contains(testType, registeredTypes);
        }
    }
}