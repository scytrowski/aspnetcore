using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<T> ReadJsonAsync<T>(this HttpRequest request)
        {
            using (StreamReader reader = new StreamReader(request.Body))
            {
                string jsonString = await reader.ReadToEndAsync();
                T obj = JsonConvert.DeserializeObject<T>(jsonString);
                return obj;
            }
        }
    }
}