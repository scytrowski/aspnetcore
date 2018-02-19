using System;
using System.Collections.Generic;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Services.Rest
{
    public interface IRestRegistry
    {
        void Register(RestRegistryEntry entry);
        IEnumerable<RestRegistryEntry> Entries { get; }
    }
}