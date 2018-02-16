using System;
using System.Collections.Generic;
using System.Linq;
using NullPointer.AspNetCore.Rest.Extensions;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Services.Rest
{
    public class RestRegistry : IRestRegistry
    {
        public void Register(Type modelType)
        {
            if (!modelType.IsRestModel())
                throw new ArgumentException("Model type must be valid rest model type");

            _registeredModelTypes.Add(modelType);
        }

        public void Register<TModel>() where TModel : RestModel
        {
            Type modelType = typeof(TModel);
            Register(modelType);
        }

        public IEnumerable<Type> RegisteredModelTypes => _registeredModelTypes.ToList().AsReadOnly();

        private readonly HashSet<Type> _registeredModelTypes = new HashSet<Type>();
    }
}