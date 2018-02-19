using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NullPointer.AspNetCore.Entity.Builders;
using NullPointer.AspNetCore.Entity.Services.Database;
using NullPointer.AspNetCore.Rest.Extensions;
using NullPointer.AspNetCore.Rest.Models;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    class ClassForRestIntegrationTest : RestModel
    {
        public string Name { get; set; }
    }

    class RestIntegrationTestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity<ClassForRestIntegrationTest>()
                .WithOnConfiguring(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddRest(contextBuilder);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRest();
        }
    }

    class RestIntegrationTestContext
    {
        public RestIntegrationTestContext()
        {
            IWebHostBuilder hostBuilder = new WebHostBuilder()
                .UseStartup<RestIntegrationTestStartup>();
            _server = new TestServer(hostBuilder);
            Client = _server.CreateClient();
        }

        public HttpClient Client { get; }

        private readonly TestServer _server;
    }

    public class RestIntegrationTest
    {
        [Fact]
        public void CheckIfGetAllReturnsValidStatusCode()
        {
            string requestUrl = $"/api/{nameof(ClassForRestIntegrationTest)}";
            HttpResponseMessage getAllResponse = _testContext.Client.GetAsync(requestUrl).Result;
            Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);
        }

        [Fact]
        public void CheckIfAddReturnsValidStatusCode()
        {
            string requestUrl = $"/api/{nameof(ClassForRestIntegrationTest)}";
            HttpContent requestContent = new StringContent(
                JsonConvert.SerializeObject(new ClassForRestIntegrationTest {
                    Name = "TestObject"
                })
            );
            HttpResponseMessage addResponse = _testContext.Client.PostAsync(requestUrl, requestContent).Result;
            Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);
        }

        private readonly RestIntegrationTestContext _testContext = new RestIntegrationTestContext();
    }
}