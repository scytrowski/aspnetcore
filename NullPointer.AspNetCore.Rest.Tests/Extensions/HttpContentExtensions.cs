using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NullPointer.AspNetCore.Rest.Tests.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            string objString = await content.ReadAsStringAsync();
            T obj = JsonConvert.DeserializeObject<T>(objString);
            return obj;
        }
    }
}