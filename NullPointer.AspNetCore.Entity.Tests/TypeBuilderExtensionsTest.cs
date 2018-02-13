using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NullPointer.AspNetCore.Entity.Extensions;
using Xunit;

namespace NullPointer.AspNetCore.Entity.Tests
{
    public class ClassForPassThroughConstructorTest
    {
        public ClassForPassThroughConstructorTest(string s, int i) {}
    }

    public class TypeBuilderExtensionsTest
    {
        [Fact]
        public void CheckIfPassThroughConstructorIsDefined()
        {
            TypeBuilder testTypeBuilder = CreateTestTypeBuilder();
            ConstructorInfo constructorToOverride = typeof(ClassForPassThroughConstructorTest)
                .GetConstructors()
                .Single(c => c.GetParameters().Length == 2);
            testTypeBuilder.DefinePassThroughConstructor(constructorToOverride);
            Type testType = testTypeBuilder.CreateType();
            ConstructorInfo definedConstructor = testType.GetConstructors()
                .SingleOrDefault(c => c.GetParameters().Length == 2);
            Assert.NotNull(definedConstructor);
        }

        [Fact]
        public void CheckIfPassThroughConstructorHasValidParameters()
        {
            TypeBuilder testTypeBuilder = CreateTestTypeBuilder();
            ConstructorInfo constructorToOverride = typeof(ClassForPassThroughConstructorTest)
                .GetConstructors()
                .Single(c => c.GetParameters().Length == 2);
            IEnumerable<Type> constructorToOverrideParameterTypes = constructorToOverride
                .GetParameters()
                .Select(p => p.ParameterType);
            testTypeBuilder.DefinePassThroughConstructor(constructorToOverride);
            Type testType = testTypeBuilder.CreateType();
            ConstructorInfo definedConstructor = testType.GetConstructors()
                .Single(c => c.GetParameters().Length == 2);
            IEnumerable<Type> definedConstructorParameterTypes = definedConstructor
                .GetParameters()
                .Select(p => p.ParameterType);
            Assert.True(definedConstructorParameterTypes
                .SequenceEqual(constructorToOverrideParameterTypes)
            );
        }

        [Fact]
        public void CheckIfTypeBuilderHasGeneratedProperty()
        {
            string testPropertyName = "TestProperty";
            Type testPropertyType = typeof(string);
            TypeBuilder testTypeBuilder = CreateTestTypeBuilder();
            testTypeBuilder.DefinePassThroughConstructors();
            PropertyBuilder testPropertyBuilder = testTypeBuilder.DefineBasicProperty(
                testPropertyName,
                testPropertyType
            );
            Type testType = testTypeBuilder.CreateType();
            PropertyInfo definedProperty = testType.GetProperties()
                .SingleOrDefault(p => p.Name == testPropertyName && p.PropertyType == testPropertyType);
            Assert.NotNull(definedProperty);
        }

        [Fact]
        public void CheckIfGeneratedPropertyHasValidName()
        {
            string testPropertyName = "TestProperty";
            TypeBuilder testTypeBuilder = CreateTestTypeBuilder();
            PropertyBuilder testPropertyBuilder = testTypeBuilder.DefineBasicProperty(
                testPropertyName,
                typeof(string)
            );
            Assert.Equal(testPropertyName, testPropertyBuilder.Name);
        }

        [Fact]
        public void CheckIfGeneratedPropertyHasValidType()
        {
            Type testPropertyType = typeof(string);
            TypeBuilder testTypeBuilder = CreateTestTypeBuilder();
            PropertyBuilder testPropertyBuilder = testTypeBuilder.DefineBasicProperty(
                "TestProperty",
                testPropertyType
            );
            Assert.Equal(testPropertyType, testPropertyBuilder.PropertyType);
        }

        private TypeBuilder CreateTestTypeBuilder()
        {
            AssemblyName assemblyName = new AssemblyName(Guid.NewGuid().ToString());
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run
            );
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(
                "TestModule"
            );
            TypeBuilder testTypeBuilder = moduleBuilder.DefineType(
                "TestClass",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(ClassForPassThroughConstructorTest)
            );
            return testTypeBuilder;
        }
    }
}