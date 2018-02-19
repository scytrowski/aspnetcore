using System;

namespace NullPointer.AspNetCore.Rest.Models
{
    public class RestRegistryEntry
    {
        public RestRegistryEntry(Type type, RestAllowedOperations allowedOperations)
        {
            Type = type;
            AllowedOperations = allowedOperations;
        }

        public Type Type { get; }
        public RestAllowedOperations AllowedOperations { get; }
    }
}