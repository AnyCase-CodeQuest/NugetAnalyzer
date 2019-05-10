using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class NugetHttpServiceTests
    {
        const string EndpointSearch = "https://api-v2v3search-1.nuget.org";
        const string EndpointPackageMetadata = "https://api.nuget.org";

        private NugetHttpService nugetHttpService;
        private Mock<HttpMessageHandler> handlerMock;

        [OneTimeSetUp]
        public void Init()
        {
            var settings = new NugetSettings
            {
                PackageMetadata = EndpointPackageMetadata,
                Search = EndpointSearch,
            };

            var optionsMock = new Mock<IOptions<NugetSettings>>();
            optionsMock
                .Setup(p => p.Value)
                .Returns(settings);

            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{'id':1,'value':'1'}]"),
                });

            var httpClient = new HttpClient(handlerMock.Object);

            nugetHttpService = new NugetHttpService(httpClient, optionsMock.Object);
        }

        [Test]
        public async Task GetPackageMetadataAsync_Should_Invoke_SendAsync()
        {
            var packageName = "NUnit";
            var version = "4.0.1";

            await nugetHttpService.GetPackageMetadataAsync(packageName, version);

            var expectedUri = new Uri($"{EndpointPackageMetadata}/v3/registration3/{packageName.ToLowerInvariant()}/{version}.json");

            handlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Get 
                            && req.RequestUri == expectedUri
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task GetDataOfPackageVersionAsync_Should_Invoke_SendAsync()
        {
            var packageName = "NUnit";

            await nugetHttpService.GetDataOfPackageVersionAsync(packageName);

            var expectedUri = new Uri($"{EndpointSearch}/query?q=PackageId:{WebUtility.UrlEncode(packageName)}&prerelease=false");

            handlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get
                        && req.RequestUri == expectedUri
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
}