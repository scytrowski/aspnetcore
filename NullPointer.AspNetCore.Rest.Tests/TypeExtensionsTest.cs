using System;
using System.Reflection;
using System.Reflection.Emit;
using NullPointer.AspNetCore.Rest.Attributes;
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
    }
}