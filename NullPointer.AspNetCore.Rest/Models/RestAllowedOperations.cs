using System;

namespace NullPointer.AspNetCore.Rest.Models
{
    [Flags]
    public enum RestAllowedOperations
    {
        None = 0,
        GetAll = 1,
        Get = 2,
        Add = 4,
        Update = 8,
        Delete = 16,
        All = 31
    }
}