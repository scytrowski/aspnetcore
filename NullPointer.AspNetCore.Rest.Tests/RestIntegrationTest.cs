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
using NullPointer.AspNetCore.Rest.Attributes;
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

    [RestDisableGetAll]
    class ClassWithDisabledGetAllForRestIntegrationTest : RestModel
    {
    }

    [RestDisableGet]
    class ClassWithDisabledGetForRestIntegrationTest : RestModel
    {
    }

    [RestDisableAdd]
    class ClassWithDisabledAddForRestIntegrationTest : RestModel
    {
    }

    [RestDisableUpdate]
    class ClassWithDisabledUpdateForRestIntegrationTest : RestModel
    {
    }

    [RestDisableDelete]
    class ClassWithDisabledDeleteForRestIntegrationTest : RestModel
    {
    }

    class RestIntegrationTestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            string dbId = Guid.NewGuid().ToString();
            DbContextBuilder contextBuilder = new DbContextBuilder()
                .WithEntity<ClassForRestIntegrationTest>()
                .WithEntity<ClassWithDisabledGetAllForRestIntegrationTest>()
                .WithEntity<ClassWithDisabledGetForRestIntegrationTest>()
                .WithEntity<ClassWithDisabledAddForRestIntegrationTest>()
                .WithEntity<ClassWithDisabledUpdateForRestIntegrationTest>()
                .WithEntity<ClassWithDisabledDeleteForRestIntegrationTest>()
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
            Server = new TestServer(hostBuilder);
            Client = Server.CreateClient();
        }

        public TestServer Server { get; }
        public HttpClient Client { get; }
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
        public void CheckIfGetAllReturnsValidStatusCodeOnDisabledEntity()
        {
            HttpResponseMessage getAllResponse = DoGetAllAsync<ClassWithDisabledGetAllForRestIntegrationTest>().Result;
            Assert.Equal(HttpStatusCode.MethodNotAllowed, getAllResponse.StatusCode);
        }

        [Fact]
        public void CheckIfGetReturnsValidStatusCode()
        {
            HttpResponseMessage addResponse = DoAddAsync(new ClassForRestIntegrationTest
            {
                Name = "TestName"
            }).Result;
            ClassForRestIntegrationTest addedModel = addResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
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
        public void CheckIfGetReturnsValidStatusCodeOnDisabledEntity()
        {
            HttpResponseMessage getResponse = DoGetAsync<ClassWithDisabledGetForRestIntegrationTest>(1).Result;
            Assert.Equal(HttpStatusCode.MethodNotAllowed, getResponse.StatusCode);
        }

        [Fact]
        public void CheckIfGetReturnsValidModel()
        {
            HttpResponseMessage addResponse = DoAddAsync(new ClassForRestIntegrationTest
            {
                Name = "TestName"
            }).Result;
            ClassForRestIntegrationTest addedModel = addResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
            int addedModelId = addedModel.Id;
            HttpResponseMessage getResponse = DoGetAsync<ClassForRestIntegrationTest>(addedModelId).Result;
            ClassForRestIntegrationTest getModel = getResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
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
        public void CheckIfAddReturnsValidStatusCodeOnDisabledEntity()
        {
            HttpResponseMessage addResponse = DoAddAsync(new ClassWithDisabledAddForRestIntegrationTest()).Result;
            Assert.Equal(HttpStatusCode.MethodNotAllowed, addResponse.StatusCode);
        }

        [Fact]
        public void CheckIfAddReturnsValidLocationHeader()
        {
            HttpResponseMessage addResponse = DoAddAsync(new ClassForRestIntegrationTest
            {
                Name = "TestName"
            }).Result;
            ClassForRestIntegrationTest addedModel = addResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
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
            ClassForRestIntegrationTest addedModel = addResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
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
        public void CheckIfUpdateReturnsValidStatusCodeOnDisabledEntity()
        {
            HttpResponseMessage updateResponse = DoUpdateAsync(new ClassWithDisabledUpdateForRestIntegrationTest()).Result;
            Assert.Equal(HttpStatusCode.MethodNotAllowed, updateResponse.StatusCode);
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

        [Fact]
        public void CheckIfDeleteReturnsValidStatusCode()
        {
            ClassForRestIntegrationTest testModel = new ClassForRestIntegrationTest
            {
                Name = "TestName"
            };
            HttpResponseMessage addResponse = DoAddAsync(testModel).Result;
            ClassForRestIntegrationTest addedModel = addResponse.Content.ReadAsJsonAsync<ClassForRestIntegrationTest>().Result;
            HttpResponseMessage deleteResponse = DoDeleteAsync(addedModel).Result;
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        [Fact]
        public void CheckIfDeleteReturnsValidStatusCodeOnDisabledEntity()
        {
            HttpResponseMessage deleteResponse = DoDeleteAsync(new ClassWithDisabledDeleteForRestIntegrationTest()).Result;
            Assert.Equal(HttpStatusCode.MethodNotAllowed, deleteResponse.StatusCode);
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
            return _testContext.Client.DeleteAsync(requestUrl, requestContent);
        }

        private readonly RestIntegrationTestContext _testContext = new RestIntegrationTestContext();
    }
}