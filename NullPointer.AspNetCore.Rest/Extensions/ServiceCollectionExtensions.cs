using Microsoft.Extensions.DependencyInjection;
using NullPointer.AspNetCore.Entity.Builders;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRest(this IServiceCollection services, DbContextBuilder contextBuilder)
        {
            
        }
    }
}