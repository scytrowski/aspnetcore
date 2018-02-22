using System;
using System.Collections.Generic;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Services.Rest
{
    public interface IRestRegistry
    {
        void Register(RestRegistryEntry entry);
        RestAllowedOperations GetAllowedOperations<TModel>() where TModel : RestModel;
        IEnumerable<RestRegistryEntry> Entries { get; }
    }
}