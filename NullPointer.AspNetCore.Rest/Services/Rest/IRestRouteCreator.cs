using Microsoft.AspNetCore.Builder;

namespace NullPointer.AspNetCore.Rest.Services.Rest
{
    public interface IRestRouteCreator
    {
        void Build(IApplicationBuilder app);
    }
}