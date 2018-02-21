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
using Microsoft.Extensions.Logging;

namespace NullPointer.AspNetCore.Rest.Tests
{
    public class ClassForRestRouterTest : RestModel
    {
    }

    public class RestRouterTest
    {
        public RestRouterTest()
        {
            Prepare();
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

        private void Prepare()
        {
            PrepareConfiguration();
            PrepareRepository();
            PrepareLogger();
            PrepareServiceProvider();
            PrepareScope();
            PrepareScopeFactory();
            PrepareRouter();
            PrepareHttpRequest();
            PrepareHttpResponse();
            PrepareHttpContext();
        }

        private void PrepareConfiguration()
        {
            _configurationMock.Setup(c => c.GetSection("restApi"))
                .Returns((IConfigurationSection)null);
        }

        private void PrepareRepository()
        {
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
        }

        private void PrepareLogger()
        {
        }

        private void PrepareServiceProvider()
        {
            ILogger<RestRouter> logger = _loggerMock.Object;
            IDataRepository<ClassForRestRouterTest> repository = _repositoryMock.Object;
            _serviceProviderMock.Setup(p => p.GetService(typeof(ILogger<RestRouter>)))
                .Returns(logger);
            _serviceProviderMock.Setup(p => p.GetService(typeof(IDataRepository<ClassForRestRouterTest>)))
                .Returns(repository);
        }

        private void PrepareScope()
        {
            IServiceProvider serviceProvider = _serviceProviderMock.Object;
            _scopeMock.Setup(s => s.ServiceProvider)
                .Returns(serviceProvider);
        }

        private void PrepareScopeFactory()
        {
            IServiceScope scope = _scopeMock.Object;
            _scopeFactoryMock.Setup(f => f.CreateScope())
                .Returns(scope);
        }

        private void PrepareRouter()
        {
            IRestRegistry restRegistry = new RestRegistry();
            restRegistry.Register(new RestRegistryEntry(typeof(ClassForRestRouterTest), RestAllowedOperations.All));
            IConfiguration configuration = _configurationMock.Object;
            IServiceScopeFactory scopeFactory = _scopeFactoryMock.Object;
            _testRouter = new RestRouter(restRegistry, configuration, scopeFactory);
        }

        private void PrepareHttpRequest()
        {
            _requestMock.Setup(r => r.Body).Returns(new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new ClassForRestRouterTest())
                )
            ));
        }

        private void PrepareHttpResponse()
        {
            _responseMock.Setup(r => r.Headers).Returns(new HeaderDictionary());
            _responseMock.Setup(r => r.Body).Returns(new MemoryStream());
            _responseMock.SetupSet(r => r.StatusCode = It.IsAny<int>());
        }

        private void PrepareHttpContext()
        {
            HttpRequest request = _requestMock.Object;
            HttpResponse response = _responseMock.Object;
            Mock<HttpContext> contextMock = new Mock<HttpContext>();
            contextMock.Setup(c => c.Request).Returns(request);
            contextMock.Setup(c => c.Response).Returns(response);
            _testContext = contextMock.Object;
        }

        private Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private Mock<IDataRepository<ClassForRestRouterTest>> _repositoryMock = new Mock<IDataRepository<ClassForRestRouterTest>>();
        private Mock<ILogger<RestRouter>> _loggerMock = new Mock<ILogger<RestRouter>>();
        private Mock<IServiceProvider> _serviceProviderMock = new Mock<IServiceProvider>();
        private Mock<IServiceScope> _scopeMock = new Mock<IServiceScope>();
        private Mock<IServiceScopeFactory> _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        private RestRouter _testRouter;
        private Mock<HttpRequest> _requestMock = new Mock<HttpRequest>();
        private Mock<HttpResponse> _responseMock = new Mock<HttpResponse>();
        private HttpContext _testContext;
    }
}