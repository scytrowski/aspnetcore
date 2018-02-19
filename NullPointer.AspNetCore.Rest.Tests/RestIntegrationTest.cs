using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
using NullPointer.AspNetCore.Rest.Tests.Extensions;
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
            string dbId = Guid.NewGuid().ToString();
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity<ClassForRestIntegrationTest>()
                .WithOnConfiguring(options => options.UseInMemoryDatabase(dbId));
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
            HttpResponseMessage getAllResponse = DoGetAllAsync<ClassForRestIntegrationTest>().Result;
            Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);
        }

        [Fact]
        public void CheckIfGetReturnsValidStatusCode()
        {
            HttpResponseMessage addResponse = DoAddAsync(new ClassForRestIntegrationTest
            {
                Name = "TestName"
            }).Result;
            ClassForRestIntegrationTest addedModel = JsonConvert.DeserializeObject<ClassForRestIntegrationTest>(
                addResponse.Content.ReadAsStringAsync().Result
            );
            int addedModelId = addedModel.Id;
            HttpResponseMessage getResponse = DoGetAsync<ClassForRestIntegrationTest>(addedModelId).Result;
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        [Fact]
        public void CheckIfGetReturnsValidStatusCodeOnNotFoundModel()
        {
            HttpResponseMessage getResponse = DoGetAsync<ClassForRestIntegrationTest>(1).Result;
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public void CheckIfGetReturnsValidModel()
        {
            HttpResponseMessage addResponse = DoAddAsync(new ClassForRestIntegrationTest
            {
                Name = "TestName"
            }).Result;
            ClassForRestIntegrationTest addedModel = JsonConvert.DeserializeObject<ClassForRestIntegrationTest>(
                addResponse.Content.ReadAsStringAsync().Result
            );
            int addedModelId = addedModel.Id;
            HttpResponseMessage getResponse = DoGetAsync<ClassForRestIntegrationTest>(addedModelId).Result;
            ClassForRestIntegrationTest getModel = JsonConvert.DeserializeObject<ClassForRestIntegrationTest>(
                getResponse.Content.ReadAsStringAsync().Result
            );
            Assert.Equal(addedModel.Id, getModel.Id);
            Assert.Equal(addedModel.Name, getModel.Name);
        }

        [Fact]
        public void CheckIfAddReturnsValidStatusCode()
        {
            HttpResponseMessage addResponse = DoAddAsync(new ClassForRestIntegrationTest
            {
                Name = "TestName"
            }).Result;
            Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);
        }

        [Fact]
        public void CheckIfAddReturnsValidLocationHeader()
        {
            HttpResponseMessage addResponse = DoAddAsync(new ClassForRestIntegrationTest
            {
                Name = "TestName"
            }).Result;
            ClassForRestIntegrationTest addedModel = JsonConvert.DeserializeObject<ClassForRestIntegrationTest>(
                addResponse.Content.ReadAsStringAsync().Result
            );
            int addedModelId = addedModel.Id;
            string addResponseLocation = addResponse.Headers.Location.ToString();
            string expectedLocation = $"/api/{nameof(ClassForRestIntegrationTest)}/{addedModelId}";
            Assert.Equal(expectedLocation, addResponseLocation);
        }

        [Fact]
        public void CheckIfAddReturnsValidModel()
        {
            ClassForRestIntegrationTest testModel = new ClassForRestIntegrationTest
            {
                Name = "TestName"
            };
            HttpResponseMessage addResponse = DoAddAsync(testModel).Result;
            ClassForRestIntegrationTest addedModel = JsonConvert.DeserializeObject<ClassForRestIntegrationTest>(
                addResponse.Content.ReadAsStringAsync().Result
            );
            Assert.NotNull(addedModel);
            Assert.Equal(testModel.Name, addedModel.Name);
        }

        [Fact]
        public void CheckIfUpdateReturnsValidStatusCode()
        {
            ClassForRestIntegrationTest testModel = new ClassForRestIntegrationTest
            {
                Name = "TestName"
            };
            HttpResponseMessage addResponse = DoAddAsync(testModel).Result;
            ClassForRestIntegrationTest addedModel = addResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
            addedModel.Name = "TestName2";
            HttpResponseMessage updateResponse = DoUpdateAsync(addedModel).Result;
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        }

        [Fact]
        public void CheckIfUpdateReturnsValidLocationHeader()
        {
            ClassForRestIntegrationTest testModel = new ClassForRestIntegrationTest
            {
                Name = "TestName"
            };
            HttpResponseMessage addResponse = DoAddAsync(testModel).Result;
            ClassForRestIntegrationTest addedModel = addResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
            addedModel.Name = "TestName2";
            HttpResponseMessage updateResponse = DoUpdateAsync(addedModel).Result;
            string updateResponseLocation = updateResponse.Headers.Location.ToString();
            string expectedLocation = $"/api/{nameof(ClassForRestIntegrationTest)}/{addedModel.Id}";
            Assert.Equal(expectedLocation, updateResponseLocation);
        }

        [Fact]
        public void CheckIfUpdateReturnsValidModel()
        {
            string newModelName = "TestName2";
            ClassForRestIntegrationTest testModel = new ClassForRestIntegrationTest
            {
                Name = "TestName"
            };
            HttpResponseMessage addResponse = DoAddAsync(testModel).Result;
            ClassForRestIntegrationTest addedModel = addResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
            addedModel.Name = newModelName;
            HttpResponseMessage updateResponse = DoUpdateAsync(addedModel).Result;
            ClassForRestIntegrationTest updatedModel = updateResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
            Assert.Equal(addedModel.Id, updatedModel.Id);
            Assert.Equal(newModelName, updatedModel.Name);
        }

        private Task<HttpResponseMessage> DoGetAllAsync<TModel>()
        {
            Type modelType = typeof(TModel);
            string requestUrl = $"/api/{modelType.Name}";
            return _testContext.Client.GetAsync(requestUrl);
        }

        private Task<HttpResponseMessage> DoGetAsync<TModel>(int id)
        {
            Type modelType = typeof(TModel);
            string requestUrl = $"/api/{modelType.Name}/{id}";
            return _testContext.Client.GetAsync(requestUrl);
        }

        private Task<HttpResponseMessage> DoAddAsync<TModel>(TModel obj)
        {
            Type modelType = typeof(TModel);
            string requestUrl = $"/api/{modelType.Name}";
            StringContent requestContent = new StringContent(
                JsonConvert.SerializeObject(obj)
            );
            return _testContext.Client.PostAsync(requestUrl, requestContent);
        }

        private Task<HttpResponseMessage> DoUpdateAsync<TModel>(TModel obj)
        {
            Type modelType = typeof(TModel);
            string requestUrl = $"/api/{modelType.Name}";
            StringContent requestContent = new StringContent(
                JsonConvert.SerializeObject(obj)
            );
            return _testContext.Client.PutAsync(requestUrl, requestContent);
        }

        private Task<HttpResponseMessage> DoDeleteAsync<TModel>(TModel obj)
        {
            Type modelType = typeof(TModel);
            string requestUrl = $"/api/{modelType.Name}";
            StringContent requestContent = new StringContent(
                JsonConvert.SerializeObject(obj)
            );
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = requestContent,
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUrl)
            };
            return _testContext.Client.SendAsync(request);
        }

        private readonly RestIntegrationTestContext _testContext = new RestIntegrationTestContext();
    }
}