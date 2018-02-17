using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class HttpResponseExtensions
    {
        public static Task WriteJsonAsync(this HttpResponse response, object obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            string objJson = JsonConvert.SerializeObject(obj);
            return response.WriteAsync(objJson, cancellationToken);
        }
    }
}