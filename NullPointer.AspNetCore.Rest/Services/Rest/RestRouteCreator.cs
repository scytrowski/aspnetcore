using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Repositories;
using NullPointer.AspNetCore.Rest.Extensions;

namespace NullPointer.AspNetCore.Rest.Services.Rest
{
    public class RestRouteCreator : IRestRouteCreator
    {
        public RestRouteCreator(IConfiguration configuration, IRestRegistry restRegistry)
        {
            Configuration = configuration;
            RestRegistry = restRegistry;
            InitializeConfiguration();
        }

        public void Build(IApplicationBuilder app)
        {
            IRouteBuilder routes = new RouteBuilder(app);
            MethodInfo buildForModelMethod = GetType()
                .GetMethod("BuildForModel");

            foreach (Type modelType in RestRegistry.RegisteredModelTypes)
            {
                buildForModelMethod.MakeGenericMethod(modelType)
                    .Invoke(this, new object[] { app, routes });
            }
        }

        public void BuildForModel<TModel>(IApplicationBuilder app, IRouteBuilder routes) where TModel : RestModel
        {
            Type modelType = typeof(TModel);
            PathString modelApiPath = _apiBasePath.Add(new PathString(modelType.Name));
            IServiceScopeFactory scopeFactory = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>();
            IRouter getAllRouter = new RouteHandler(async context => 
            {
                if (!HttpMethods.IsGet(context.Request.Method) || context.Request.Path != modelApiPath)
                    return;

                using (IServiceScope scope = scopeFactory.CreateScope())
                {
                    IDataRepository<TModel> repository = scope.ServiceProvider
                        .GetRequiredService<IDataRepository<TModel>>();
                    IEnumerable<TModel> models = await repository.GetAllAsync();
                    await context.Response.WriteJsonAsync(models);
                }
            });
        }

        private void ProvideDefaultRestApiConfiguration()
        {
            _apiBasePath = new PathString("/api");
        }

        private void InitializeConfiguration()
        {
            IConfigurationSection restApiSection = Configuration.GetSection("restApi");

            if (restApiSection != null)
            {
                string restApiBasePathString = restApiSection.GetValue<string>("basePath");
                _apiBasePath = new PathString(restApiBasePathString);
            }
            else
                ProvideDefaultRestApiConfiguration();
        }

        public IConfiguration Configuration { get; }
        public IRestRegistry RestRegistry { get; }

        private PathString _apiBasePath;
    }
}