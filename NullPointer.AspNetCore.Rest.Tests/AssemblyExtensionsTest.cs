using System;
using System.Reflection;
using System.Reflection.Emit;
using NullPointer.AspNetCore.Rest.Attributes;
using NullPointer.AspNetCore.Rest.Extensions;
using NullPointer.AspNetCore.Rest.Models;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    public class AssemblyExtensionsTest
    {
        public AssemblyExtensionsTest()
        {
            CreateTestAssembly();
        }

        [Fact]
        public void CheckIfValidTypesDetected()
        {
            Type[] restModelTypes = _testAssembly.GetRestModelTypes();
            Assert.Contains(_testRestModelType, restModelTypes);
            Assert.DoesNotContain(_testRestModelWithRestIgnoreType, restModelTypes);
            Assert.DoesNotContain(_abstractTestRestModelType, restModelTypes);
        }

        private void CreateTestAssembly()
        {
            AssemblyName assemblyName = new AssemblyName(Guid.NewGuid().ToString());
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run
            );
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("TestModule");
            TypeBuilder testRestModelTypeBuilder = moduleBuilder.DefineType(
                "TestRestModel",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(RestModel)
            );
            _testRestModelType = testRestModelTypeBuilder.CreateType();
            TypeBuilder testRestModelWithRestIgnoreTypeBuilder = moduleBuilder.DefineType(
                "TestRestModelWithRestIgnore",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(RestModel)
            );
            CustomAttributeBuilder restIgnoreAttributeBuilder = new CustomAttributeBuilder(
                typeof(RestIgnoreAttribute).GetConstructor(new Type[] {}),
                new object[] {}
            );
            testRestModelWithRestIgnoreTypeBuilder.SetCustomAttribute(restIgnoreAttributeBuilder);
            _testRestModelWithRestIgnoreType = testRestModelWithRestIgnoreTypeBuilder.CreateType();
            TypeBuilder abstractTestRestModelTypeBuilder = moduleBuilder.DefineType(
                "AbstractTestRestModel",
                TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Class,
                typeof(RestModel)
            );
            _abstractTestRestModelType = abstractTestRestModelTypeBuilder.CreateType();
            _testAssembly = moduleBuilder.Assembly;
        }

        private Type _testRestModelType;
        private Type _testRestModelWithRestIgnoreType;
        private Type _abstractTestRestModelType;
        private Assembly _testAssembly;
    }
}