using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NullPointer.AspNetCore.Rest.Models;
using NullPointer.AspNetCore.Rest.Services.Repositories;
using NullPointer.AspNetCore.Rest.Services.Rest;
using NullPointer.AspNetCore.Rest.Attributes;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    public class ClassForRestRouterTest : RestModel
    {
    }

    public class RestRouterTest
    {
        public RestRouterTest()
        {
            PrepareRouter();
            PrepareHttpContext();
        }

        [Fact]
        public void CheckIfGetAllAsyncInvokedInGetAllHandler()
        {
            RequestDelegate getAllHandler = _testRouter.CreateGetAllHandler<ClassForRestRouterTest>();
            getAllHandler.Invoke(_testContext).Wait();
            _repositoryMock.Verify(r => r.GetAllAsync());
        }

        [Fact]
        public void CheckIfGetAllHandlerSetsValidStatusCode()
        {
            RequestDelegate getAllHandler = _testRouter.CreateGetAllHandler<ClassForRestRouterTest>();
            getAllHandler.Invoke(_testContext).Wait();
            _responseMock.VerifySet(r => r.StatusCode = StatusCodes.Status200OK);
        }

        [Fact]
        public void CheckIfGetAsyncInvokedInGetHandler()
        {
            int testId = 5;
            RequestDelegate getHandler = _testRouter.CreateGetHandler<ClassForRestRouterTest>(testId);
            getHandler.Invoke(_testContext).Wait();
            _repositoryMock.Verify(r => r.GetAsync(testId));
        }

        [Fact]
        public void CheckIfGetHandlerSetsValidStatusCode()
        {
            RequestDelegate getHandler = _testRouter.CreateGetHandler<ClassForRestRouterTest>(5);
            getHandler.Invoke(_testContext).Wait();
            _responseMock.VerifySet(r => r.StatusCode = StatusCodes.Status200OK);
        }

        [Fact]
        public void CheckIfAddAsyncInvokedInAddHandler()
        {
            RequestDelegate addHandler = _testRouter.CreateAddHandler<ClassForRestRouterTest>();
            addHandler.Invoke(_testContext).Wait();
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ClassForRestRouterTest>()));
        }

        [Fact]
        public void CheckIfAddHandlerSetsValidStatusCode()
        {
            RequestDelegate addHandler = _testRouter.CreateAddHandler<ClassForRestRouterTest>();
            addHandler.Invoke(_testContext).Wait();
            _responseMock.VerifySet(r => r.StatusCode = StatusCodes.Status201Created);
        }

        [Fact]
        public void CheckIfUpdateAsyncInvokedInUpdateHandler()
        {
            RequestDelegate updateHandler = _testRouter.CreateUpdateHandler<ClassForRestRouterTest>();
            updateHandler.Invoke(_testContext).Wait();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<ClassForRestRouterTest>()));
        }

        [Fact]
        public void CheckIfUpdateHandlerSetsValidStatusCode()
        {
            RequestDelegate updateHandler = _testRouter.CreateUpdateHandler<ClassForRestRouterTest>();
            updateHandler.Invoke(_testContext).Wait();
            _responseMock.VerifySet(r => r.StatusCode = StatusCodes.Status200OK);
        }

        [Fact]
        public void CheckIfDeleteAsyncInvokedInDeleteHandler()
        {
            RequestDelegate deleteHandler = _testRouter.CreateDeleteHandler<ClassForRestRouterTest>();
            deleteHandler.Invoke(_testContext).Wait();
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<ClassForRestRouterTest>()));
        }

        [Fact]
        public void CheckIfDeleteHandlerSetsValidStatusCode()
        {
            RequestDelegate deleteHandler = _testRouter.CreateDeleteHandler<ClassForRestRouterTest>();
            deleteHandler.Invoke(_testContext).Wait();
            _responseMock.VerifySet(r => r.StatusCode = StatusCodes.Status200OK);
        }

        private void PrepareRouter()
        {
            Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c.GetSection("restApi"))
                .Returns((IConfigurationSection)null);
            IConfiguration configuration = configurationMock.Object;
            _repositoryMock.Setup(r => r.GetAllAsync())
                .Returns(Task.FromResult(Enumerable.Empty<ClassForRestRouterTest>()));
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(new ClassForRestRouterTest()));
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<ClassForRestRouterTest>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ClassForRestRouterTest>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<ClassForRestRouterTest>()))
                .Returns(Task.CompletedTask);
            IDataRepository<ClassForRestRouterTest> repository = _repositoryMock.Object;
            IRestRegistry restRegistry = new RestRegistry();
            restRegistry.Register(new RestRegistryEntry(typeof(ClassForRestRouterTest), RestAllowedOperations.All));
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(IDataRepository<ClassForRestRouterTest>)))
                .Returns(repository);
            IServiceProvider serviceProvider = serviceProviderMock.Object;
            Mock<IServiceScope> scopeMock = new Mock<IServiceScope>();
            scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProvider);
            IServiceScope scope = scopeMock.Object;
            Mock<IServiceScopeFactory> scopeFactoryMock = new Mock<IServiceScopeFactory>();
            scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scope);
            IServiceScopeFactory scopeFactory = scopeFactoryMock.Object;
            _testRouter = new RestRouter(restRegistry, configuration, scopeFactory);
        }

        private void PrepareHttpContext()
        {
            Mock<HttpRequest> requestMock = new Mock<HttpRequest>();
            requestMock.Setup(r => r.Body).Returns(new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new ClassForRestRouterTest())
                )
            ));
            HttpRequest request = requestMock.Object;
            _responseMock = new Mock<HttpResponse>();
            _responseMock.Setup(r => r.Headers).Returns(new HeaderDictionary());
            _responseMock.Setup(r => r.Body).Returns(new MemoryStream());
            _responseMock.SetupSet(r => r.StatusCode = It.IsAny<int>());
            HttpResponse response = _responseMock.Object;
            Mock<HttpContext> contextMock = new Mock<HttpContext>();
            contextMock.Setup(c => c.Request).Returns(request);
            contextMock.Setup(c => c.Response).Returns(response);
            _testContext = contextMock.Object;
        }

        private Mock<IDataRepository<ClassForRestRouterTest>> _repositoryMock = new Mock<IDataRepository<ClassForRestRouterTest>>();
        private RestRouter _testRouter;
        private Mock<HttpResponse> _responseMock = new Mock<HttpResponse>();
        private HttpContext _testContext;
    }
}