using System;
using System.Reflection;
using System.Reflection.Emit;
using NullPointer.AspNetCore.Rest.Attributes;
using NullPointer.AspNetCore.Rest.Builders;
using NullPointer.AspNetCore.Rest.Extensions;
using NullPointer.AspNetCore.Rest.Models;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    class ClassForIsRestModelTestType : RestModel
    {
    }

    [RestIgnore]
    class ClassForIsRestModelTestTypeWithRestIgnore : RestModel
    {
    }

    abstract class ClassForIsRestModelTestAbstractType : RestModel
    {
    }

    [RestDisableGetAll]
    class ClassWithDisabledGetAllForGetRestAllowedOperationsTest : RestModel
    {
    }

    [RestDisableGet]
    class ClassWithDisabledGetForGetRestAllowedOperationsTest : RestModel
    {
    }

    [RestDisableAdd]
    class ClassWithDisabledAddForGetRestAllowedOperationsTest : RestModel
    {
    }

    [RestDisableUpdate]
    class ClassWithDisabledUpdateForGetRestAllowedOperationsTest : RestModel
    {
    }

    [RestDisableDelete]
    class ClassWithDisabledDeleteForGetRestAllowedOperationsTest : RestModel
    {
    }

    public class TypeExtensionsTest
    {
        [Fact]
        public void CheckIfTrueForRestModelSubtype()
        {
            Type testType = typeof(ClassForIsRestModelTestType);
            Assert.True(testType.IsRestModel());
        }        

        [Fact]
        public void CheckIfFalseForRestModelSubtypeWithRestIgnore()
        {
            Type testType = typeof(ClassForIsRestModelTestTypeWithRestIgnore);
            Assert.False(testType.IsRestModel());
        }

        [Fact]
        public void CheckIfFalseForAbstractType()
        {
            Type testType = typeof(ClassForIsRestModelTestAbstractType);
            Assert.False(testType.IsRestModel());
        }

        [Fact]
        public void CheckIfHasValidRestAllowedOperationsWithDisabledGetAll()
        {
            RestAllowedOperations expectedAllowedOperations = new RestModelOptionsBuilder()
                .WithDisabledGetAll()
                .AllowedOperations;
            RestAllowedOperations testAllowedOperations = typeof(ClassWithDisabledGetAllForGetRestAllowedOperationsTest)
                .GetRestAllowedOperations();
            Assert.Equal(expectedAllowedOperations, testAllowedOperations);
        }

        [Fact]
        public void CheckIfHasValidRestAllowedOperationsWithDisabledGet()
        {
            RestAllowedOperations expectedAllowedOperations = new RestModelOptionsBuilder()
                .WithDisabledGet()
                .AllowedOperations;
            RestAllowedOperations testAllowedOperations = typeof(ClassWithDisabledGetForGetRestAllowedOperationsTest)
                .GetRestAllowedOperations();
            Assert.Equal(expectedAllowedOperations, testAllowedOperations);
        }

        [Fact]
        public void CheckIfHasValidRestAllowedOperationsWithDisabledAdd()
        {
            RestAllowedOperations expectedAllowedOperations = new RestModelOptionsBuilder()
                .WithDisabledAdd()
                .AllowedOperations;
            RestAllowedOperations testAllowedOperations = typeof(ClassWithDisabledAddForGetRestAllowedOperationsTest)
                .GetRestAllowedOperations();
            Assert.Equal(expectedAllowedOperations, testAllowedOperations);
        }

        [Fact]
        public void CheckIfHasValidRestAllowedOperationsWithDisabledUpdate()
        {
            RestAllowedOperations expectedAllowedOperations = new RestModelOptionsBuilder()
                .WithDisabledUpdate()
                .AllowedOperations;
            RestAllowedOperations testAllowedOperations = typeof(ClassWithDisabledUpdateForGetRestAllowedOperationsTest)
                .GetRestAllowedOperations();
            Assert.Equal(expectedAllowedOperations, testAllowedOperations);
        }

        [Fact]
        public void CheckIfHasValidRestAllowedOperationsWithDisabledDelete()
        {
            RestAllowedOperations expectedAllowedOperations = new RestModelOptionsBuilder()
                .WithDisabledDelete()
                .AllowedOperations;
            RestAllowedOperations testAllowedOperations = typeof(ClassWithDisabledDeleteForGetRestAllowedOperationsTest)
                .GetRestAllowedOperations();
            Assert.Equal(expectedAllowedOperations, testAllowedOperations);
        }
    }
}