using System;
using System.Linq;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Rest;

namespace NullPointer.AspNetCore.Rest.Builders
{
    public class RestOptionsBuilder
    {
        public RestOptionsBuilder(IRestRegistry registry)
        {
            _registry = registry;
        }

        public RestOptionsBuilder WithModelOptions<TModel>(Action<RestModelOptionsBuilder> buildAction) where TModel : RestModel
        {
            Type modelType = typeof(TModel);
            RestRegistryEntry modelTypeEntry = _registry.Entries
                .SingleOrDefault(e => e.Type == modelType);
            
            if (modelTypeEntry == null)
                throw new InvalidOperationException("Provided model type is not registered in REST registry");

            RestModelOptionsBuilder options = new RestModelOptionsBuilder(modelTypeEntry.AllowedOperations);
            buildAction?.Invoke(options);
            RestRegistryEntry newModelTypeEntry = new RestRegistryEntry(modelType, options.AllowedOperations);
            _registry.Register(newModelTypeEntry);
            return this;
        }

        private readonly IRestRegistry _registry;
    }
}