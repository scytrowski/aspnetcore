using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NullPointer.AspNetCore.Rest.Services.Rest;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseRest(this IApplicationBuilder app)
        {
            IRestRouteCreator routeCreator = app.ApplicationServices
                .GetRequiredService<IRestRouteCreator>();
            routeCreator.Build(app);
        }
    }
}