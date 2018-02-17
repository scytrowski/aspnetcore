using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NullPointer.AspNetCore.Rest.Extensions;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Repositories;

namespace NullPointer.AspNetCore.Rest.Services.Rest
{
    public class RestRouter : IRouter
    {
        public RestRouter(IRestRegistry restRegistry, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            RestRegistry = restRegistry;
            Configuration = configuration;
            ScopeFactory = scopeFactory;
            InitializeConfiguration();
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            HandleRequest(context);
            return Task.CompletedTask;
        }

        public RequestDelegate CreateGetAllHandler<TModel>() where TModel : RestModel
        {
            return async context => 
            {
                using (IServiceScope scope = ScopeFactory.CreateScope())
                {
                    IDataRepository<TModel> repository = scope.ServiceProvider
                        .GetRequiredService<IDataRepository<TModel>>(); 
                    IEnumerable<TModel> models = await repository.GetAllAsync();
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteJsonAsync(models);
                }
            };
        }

        public RequestDelegate CreateAddHandler<TModel>() where TModel : RestModel
        {
            return async context =>
            {
                TModel model = await context.Request.ReadJsonAsync<TModel>();

                using (IServiceScope scope = ScopeFactory.CreateScope())
                {
                    IDataRepository<TModel> repository = scope.ServiceProvider
                        .GetRequiredService<IDataRepository<TModel>>();
                    await repository.AddAsync(model);
                    context.Response.StatusCode = StatusCodes.Status201Created;
                }
            };
        }

        public RequestDelegate CreateUpdateHandler<TModel>() where TModel : RestModel
        {
            return async context =>
            {
                TModel model = await context.Request.ReadJsonAsync<TModel>();

                using (IServiceScope scope = ScopeFactory.CreateScope())
                {
                    IDataRepository<TModel> repository = scope.ServiceProvider
                        .GetRequiredService<IDataRepository<TModel>>();
                    await repository.UpdateAsync(model);
                    context.Response.StatusCode = StatusCodes.Status200OK;
                }
            };
        }

        public RequestDelegate CreateDeleteHandler<TModel>() where TModel : RestModel
        {
            return async context =>
            {
                TModel model = await context.Request.ReadJsonAsync<TModel>();

                using (IServiceScope scope = ScopeFactory.CreateScope())
                {
                    IDataRepository<TModel> repository = scope.ServiceProvider
                        .GetRequiredService<IDataRepository<TModel>>();
                    await repository.DeleteAsync(model);
                    context.Response.StatusCode = StatusCodes.Status200OK;
                } 
            };
        }

        private RequestDelegate CreateRequestHandler(string requestMethod, Type requestModelType)
        {
            if (HttpMethods.IsGet(requestMethod))
            {
                return GetType().GetMethod(nameof(CreateGetAllHandler))
                    .MakeGenericMethod(requestModelType)
                    .Invoke(this, new object[] {}) as RequestDelegate;
            }
            else if (HttpMethods.IsPost(requestMethod))
            {
                return GetType().GetMethod(nameof(CreateAddHandler))
                    .MakeGenericMethod(requestModelType)
                    .Invoke(this, new object[] {}) as RequestDelegate;
            }
            else if (HttpMethods.IsPut(requestMethod))
            {
                return GetType().GetMethod(nameof(CreateUpdateHandler))
                    .MakeGenericMethod(requestModelType)
                    .Invoke(this, new object[] {}) as RequestDelegate;
            }
            else if (HttpMethods.IsDelete(requestMethod))
            {
                return GetType().GetMethod(nameof(CreateDeleteHandler))
                    .MakeGenericMethod(requestModelType)
                    .Invoke(this, new object[] {}) as RequestDelegate;
            }

            return null;
        }

        private void HandleRequest(RouteContext context)
        {
            PathString requestPath = context.HttpContext.Request.Path;
            string requestMethod = context.HttpContext.Request.Method;
            
            if (!requestPath.StartsWithSegments(_apiBasePath))
                return;

            Type requestModelType = null;

            foreach (KeyValuePair<Type, PathString> modelApiRoutePair in _modelApiRoutes)
            {
                if (requestPath.StartsWithSegments(modelApiRoutePair.Value))
                {
                    requestModelType = modelApiRoutePair.Key;
                    break;
                }
            }

            if (requestModelType == null)
                return;

            context.Handler = CreateRequestHandler(requestMethod, requestModelType);
        }

        private void ProvideDefaultConfiguration()
        {
            _apiBasePath = new PathString("/api");
        }

        private void InitializeConfiguration()
        {
            IConfigurationSection apiConfigurationSection = Configuration.GetSection("restApi");

            if (apiConfigurationSection != null)
                _apiBasePath = new PathString(apiConfigurationSection.GetValue<string>("basePath"));
            else
                ProvideDefaultConfiguration();

            foreach (Type modelType in RestRegistry.RegisteredModelTypes)
                _modelApiRoutes[modelType] = _apiBasePath.SafeAdd(modelType.Name);
        }

        public IRestRegistry RestRegistry { get; }
        public IConfiguration Configuration { get; }
        public IServiceScopeFactory ScopeFactory { get; }

        private PathString _apiBasePath;
        private Dictionary<Type, PathString> _modelApiRoutes = new Dictionary<Type, PathString>();
    }
}