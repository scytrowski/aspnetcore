using System;
using System.Collections.Generic;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Services.Rest
{
    public interface IRestRegistry
    {
        void Register(Type modelType);
        void Register<TModel>() where TModel : RestModel;
        IEnumerable<Type> RegisteredModelTypes { get; }
    }
}