using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullPointer.AspNetCore.Rest.Extensions;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Repositories;

namespace NullPointer.AspNetCore.Rest.Services.Rest
{
    public class RestRouter : IRestRouter
    {
        public RestRouter(IRestRegistry restRegistry, IConfiguration configuration, IServiceScopeFactory scopeFactory, ILogger<RestRouter> logger)
        {
            RestRegistry = restRegistry;
            Configuration = configuration;
            ScopeFactory = scopeFactory;
            Logger = logger;
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
            return context => 
            {
                return DoInScopeAsync<TModel>(async repository =>
                {
                    Logger.LogDebug(RestLoggingEvents.GET_ALL_REQUEST, "Requested GET ALL method on '{MODEL}' model", typeof(TModel).Name);

                    if (!IsOperationAllowed<TModel>(RestAllowedOperations.GetAll))
                    {
                        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        Logger.LogDebug(RestLoggingEvents.DISABLED_METHOD_REQUEST, "Requested disabled GET ALL method on '{MODEL}' model", typeof(TModel).Name);
                        return;
                    }

                    IEnumerable<TModel> models = await repository.GetAllAsync();
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteJsonAsync(models);


                });
            };
        }

        public RequestDelegate CreateGetHandler<TModel>(int? id) where TModel : RestModel
        {
            return context =>
            {
                return DoInScopeAsync<TModel>(async repository =>
                {
                    Logger.LogDebug(RestLoggingEvents.GET_REQUEST, "Requested GET method on '{MODEL}' model", typeof(TModel).Name);

                    if (!IsOperationAllowed<TModel>(RestAllowedOperations.Get))
                    {
                        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        Logger.LogDebug(RestLoggingEvents.DISABLED_METHOD_REQUEST, "Requested disabled GET method on '{MODEL}' model", typeof(TModel).Name);
                        return;
                    } 
                    else if (id == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }

                    TModel model = await repository.GetAsync(id.Value);

                    if (model != null)
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        await context.Response.WriteJsonAsync(model);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                    }
                });
            };
        }

        public RequestDelegate CreateAddHandler<TModel>() where TModel : RestModel
        {
            return context =>
            {
                return DoInScopeAsync<TModel>(async repository =>
                {
                    Logger.LogDebug(RestLoggingEvents.ADD_REQUEST, "Requested ADD method on '{MODEL}' model", typeof(TModel).Name);

                    if (!IsOperationAllowed<TModel>(RestAllowedOperations.Add))
                    {
                        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        Logger.LogDebug(RestLoggingEvents.DISABLED_METHOD_REQUEST, "Requested disabled ADD method on '{MODEL}' model", typeof(TModel).Name);
                        return;
                    }

                    TModel model = await context.Request.ReadJsonAsync<TModel>();
                    await repository.AddAsync(model);
                    string modelLocation = GetModelLocation(model);
                    context.Response.StatusCode = StatusCodes.Status201Created;
                    context.Response.Headers.Add("location", modelLocation);
                    await context.Response.WriteJsonAsync(model);
                });
            };
        }

        public RequestDelegate CreateUpdateHandler<TModel>() where TModel : RestModel
        {
            return context =>
            {
                return DoInScopeAsync<TModel>(async repository =>
                {
                    Logger.LogDebug(RestLoggingEvents.UPDATE_REQUEST, "Requested UPDATE method on '{MODEL}' model", typeof(TModel).Name);

                    if (!IsOperationAllowed<TModel>(RestAllowedOperations.Update))
                    {
                        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        Logger.LogDebug(RestLoggingEvents.DISABLED_METHOD_REQUEST, "Requested disabled UPDATE method on '{MODEL}' model", typeof(TModel).Name);
                        return;
                    }

                    TModel model = await context.Request.ReadJsonAsync<TModel>();
                    await repository.UpdateAsync(model);
                    string modelLocation = GetModelLocation(model);
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.Headers.Add("location", modelLocation);
                    await context.Response.WriteJsonAsync(model);
                });
            };
        }

        public RequestDelegate CreateDeleteHandler<TModel>() where TModel : RestModel
        {
            return context =>
            {
                return DoInScopeAsync<TModel>(async repository =>
                {
                    Logger.LogDebug(RestLoggingEvents.DELETE_REQUEST, "Requested DELETE method on '{MODEL}' model", typeof(TModel).Name);

                    if (!IsOperationAllowed<TModel>(RestAllowedOperations.Delete))
                    {
                        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        Logger.LogDebug(RestLoggingEvents.DISABLED_METHOD_REQUEST, "Requested disabled DELETE method on '{MODEL}' model", typeof(TModel).Name);
                        return;
                    }

                    TModel model = await context.Request.ReadJsonAsync<TModel>();
                    await repository.DeleteAsync(model);
                    context.Response.StatusCode = StatusCodes.Status200OK;
                });
            };
        }

        private RequestDelegate CreateRequestHandler(string requestMethod, Type requestModelType, bool hasIdSegment, int? id)
        {
            if (HttpMethods.IsGet(requestMethod))
            {
                if (hasIdSegment)
                {
                    return GetType().GetMethod(nameof(CreateGetHandler))
                        .MakeGenericMethod(requestModelType)
                        .Invoke(this, new object[] { id }) as RequestDelegate;
                }
                else
                {
                    return GetType().GetMethod(nameof(CreateGetAllHandler))
                        .MakeGenericMethod(requestModelType)
                        .Invoke(this, new object[] {}) as RequestDelegate;
                }
            }

            if (hasIdSegment)
                return null;

            if (HttpMethods.IsPost(requestMethod))
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

            PathString modelApiPath = _modelApiRoutes[requestModelType];
            string[] requestPathSegments = requestPath.GetSegments();
            int segmentsCountDiff = requestPathSegments.Length - modelApiPath.GetSegments().Length;
            bool hasIdSegment;
            int? idNullable = null;

            if (segmentsCountDiff == 0)
                hasIdSegment = false;
            else if (segmentsCountDiff == 1)
            {
                hasIdSegment = true;

                if (int.TryParse(requestPathSegments.Last(), out int id))
                    idNullable = id;
            }
            else
                return;

            context.Handler = CreateRequestHandler(requestMethod, requestModelType, hasIdSegment, idNullable);
        }

        private bool IsOperationAllowed<TModel>(RestAllowedOperations operation) where TModel: RestModel
        {
            RestAllowedOperations modelAllowedOperations = RestRegistry.GetAllowedOperations<TModel>();
            return (modelAllowedOperations & operation) == operation;
        }

        private string GetModelLocation<TModel>(TModel model) where TModel : RestModel
        {
            Type modelType = typeof(TModel);

            if (!_modelApiRoutes.ContainsKey(modelType))
                throw new InvalidOperationException("Provided model type must be registered");
            
            PathString modelPath = _modelApiRoutes[modelType].SafeAdd(model.Id.ToString());
            return modelPath.Value;
        }

        private Task DoInScopeAsync<TModel>(Func<IDataRepository<TModel>, Task> action) where TModel : RestModel
        {
            return Task.Run(() =>
            {
                try
                {
                    using (IServiceScope scope = ScopeFactory.CreateScope())
                    {
                        IDataRepository<TModel> repository = scope.ServiceProvider
                            .GetRequiredService<IDataRepository<TModel>>();
                        action?.Invoke(repository).Wait();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(RestLoggingEvents.SERVER_FAILURE, ex, "An exception has been raised while handling request");
                }
            });
        }

        private void ProvideDefaultConfiguration()
        {
            _apiBasePath = new PathString("/api");
        }

        private void InitializeConfiguration()
        {
            IConfigurationSection apiConfigurationSection = Configuration.GetSection("restApi");

            if (apiConfigurationSection?.Value != null)
                _apiBasePath = new PathString(apiConfigurationSection.GetValue<string>("basePath"));
            else
                ProvideDefaultConfiguration();

            foreach (RestRegistryEntry entry in RestRegistry.Entries)
            {
                Type entryType = entry.Type;
                _modelApiRoutes[entryType] = _apiBasePath.SafeAdd(entryType.Name);
            }
        }

        public IRestRegistry RestRegistry { get; }
        public IConfiguration Configuration { get; }
        public IServiceScopeFactory ScopeFactory { get; }
        public ILogger<RestRouter> Logger { get; }

        private PathString _apiBasePath;
        private readonly Dictionary<Type, PathString> _modelApiRoutes = new Dictionary<Type, PathString>();
    }
}