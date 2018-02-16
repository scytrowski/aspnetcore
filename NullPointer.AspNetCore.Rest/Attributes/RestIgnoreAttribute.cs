using System;

namespace NullPointer.AspNetCore.Rest.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RestIgnoreAttribute : Attribute
    {
    }
}