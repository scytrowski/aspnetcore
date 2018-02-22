using System;
using System.Collections.Generic;
using System.Linq;
using NullPointer.AspNetCore.Rest.Extensions;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Services.Rest
{
    public class RestRegistry : IRestRegistry
    {
        public void Register(RestRegistryEntry entry)
        {
            if (!entry.Type.IsRestModel())
                throw new ArgumentException("Entry type must be a valid subtype of RestModel");

            _entryDictionary[entry.Type] = entry;
        }

        public RestAllowedOperations GetAllowedOperations<TModel>() where TModel : RestModel
        {
            Type modelType = typeof(TModel);

            if (!_entryDictionary.ContainsKey(modelType))
                throw new InvalidOperationException("Provided model type must be registered");

            return _entryDictionary[modelType].AllowedOperations;
        }

        public IEnumerable<RestRegistryEntry> Entries => _entryDictionary.Values.AsEnumerable();

        private readonly Dictionary<Type, RestRegistryEntry> _entryDictionary = new Dictionary<Type, RestRegistryEntry>();
    }
}