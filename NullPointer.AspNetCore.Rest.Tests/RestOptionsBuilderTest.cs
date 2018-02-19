using System;
using System.Linq;
using Moq;
using NullPointer.AspNetCore.Rest.Builders;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Rest;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    class ClassForRestOptionsBuilderTest : RestModel
    {
    }

    public class RestOptionsBuilderTest
    {
        [Fact]
        public void CheckIfRegistryEntryIsUpdated()
        {
            Mock<IRestRegistry> registryMock = new Mock<IRestRegistry>();
            registryMock.Setup(r => r.Entries)
                .Returns(new RestRegistryEntry[] {
                    new RestRegistryEntry(
                        typeof(ClassForRestOptionsBuilderTest), 
                        RestAllowedOperations.All)   
                });
            registryMock.Setup(r => r.Register(It.IsAny<RestRegistryEntry>()));
            IRestRegistry registry = registryMock.Object;
            RestOptionsBuilder options = new RestOptionsBuilder(registry)
                .WithModelOptions<ClassForRestOptionsBuilderTest>(modelOptions => 
                {
                    modelOptions.WithDisabledAdd();
                });
            registryMock.Verify(r => r.Register(It.IsAny<RestRegistryEntry>()), Times.Once);
        }
    }
}