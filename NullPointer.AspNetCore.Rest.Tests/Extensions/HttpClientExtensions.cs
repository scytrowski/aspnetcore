using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NullPointer.AspNetCore.Rest.Extensions;

namespace NullPointer.AspNetCore.Rest.Tests.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string requestUri, HttpContent requestContent)
        {
            string realRequestUri = client.BaseAddress.ToString().TrimEnd('/') + requestUri;
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Content = requestContent,
                Method = HttpMethod.Delete,
                RequestUri = new Uri(realRequestUri)
            };
            return client.SendAsync(requestMessage);
        }
    }
}