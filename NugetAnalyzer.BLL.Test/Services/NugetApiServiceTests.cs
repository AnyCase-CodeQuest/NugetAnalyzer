using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Domain;
using NUnit.Framework;
using SemanticComparison;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class NugetApiServiceTests
    {
        private IConfiguration configuration;
        private INugetApiService nugetApiService;
        private FluentMockServer server;

        [OneTimeSetUp]
        public void Init()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(Environment.CurrentDirectory))
                .AddJsonFile("appsettings.json", false)
                .Build();

            nugetApiService = new NugetApiService(configuration);

            server = FluentMockServer
                .Start(port: 50000);
        }

        [TearDown]
        public void Reset()
        {
            server.Reset();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            server.Stop();
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Latest_Version_Of_Package_When_Package_Correct()
        {
            server
                .Given(Request
                    .Create()
                    .WithPath("/query")
                    .UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyFromFile(Path.GetFullPath(configuration["EndpointSearch:NUnit:Response"])));

            var packageVersion = await nugetApiService
                .GetLatestVersionPackageAsync(configuration["EndpointSearch:NUnit:PackageName"]);

            var expectedVersion = new Likeness<PackageVersion, PackageVersion>(packageVersion);

            Assert.AreEqual(expectedVersion, new PackageVersion
            {
                Major = 3,
                Minor = 11,
                Build = 0,
                Revision = -1
            });
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Package_Incorrect()
        {
            server
                .Given(Request
                    .Create()
                    .WithPath("/query")
                    .UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyFromFile(Path.GetFullPath(configuration["EndpointSearch:NotExist:Response"])));

            var packageVersion = await nugetApiService
                .GetLatestVersionPackageAsync(configuration["EndpointSearch:NotExist:PackageName"]);

            Assert.AreEqual(packageVersion, null);
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Response_Not_Json()
        {
            server
                .Given(Request
                    .Create()
                    .WithPath("/query")
                    .UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(500)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(""));

            var packageVersion = await nugetApiService
                .GetLatestVersionPackageAsync(configuration["EndpointSearch:NUnit:PackageName"]);

            Assert.AreEqual(packageVersion, null);
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Version_Of_Package_Incorrect()
        {
            server
                .Given(Request
                    .Create()
                    .WithPath("/query")
                    .UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(500)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyFromFile(Path.GetFullPath(configuration["EndpointSearch:VersionIncorrect:Response"])));

            var packageVersion = await nugetApiService
                .GetLatestVersionPackageAsync(configuration["EndpointSearch:VersionIncorrect:PackageName"]);

            Assert.AreEqual(packageVersion, null);
        }

        [Test]
        public void GetLatestVersionPackageAsync_Should_Throw_ArgumentNullException_When_Package_Name_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await nugetApiService.GetLatestVersionPackageAsync(null));
        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_DateTime_When_Parameters_Correct()
        {
            server
                .Given(Request
                    .Create()
                    .WithPath("/v3/registration3/*")
                    .UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyFromFile(Path.GetFullPath(configuration["EndpointPackageMetadata:NUnit:Response"])));


            var publishedDate = await nugetApiService.GetPublishedDateByVersionAsync(
                    configuration["EndpointPackageMetadata:NUnit:PackageName"],
                    configuration["EndpointPackageMetadata:NUnit:Version"]);

            Assert.IsInstanceOf<DateTime?>(publishedDate);
            Assert.AreEqual(publishedDate, DateTime
                                                        .Parse(configuration["EndpointPackageMetadata:NUnit:Published"])
                                                        .ToUniversalTime());
        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_Null_When_Parameters_Incorrect()
        {
            server
                .Given(Request
                    .Create()
                    .WithPath("/v3/registration3/*")
                    .UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(404)
                        .WithHeader("Content-Type", "application/xml")
                        .WithBodyFromFile(Path.GetFullPath(configuration["EndpointPackageMetadata:NotFound:Response"])));

            var publishedDate = await nugetApiService.GetPublishedDateByVersionAsync(
                configuration["EndpointPackageMetadata:NotFound:PackageName"],
                configuration["EndpointPackageMetadata:NotFound:Version"]);

            Assert.AreEqual(publishedDate, null);
        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_Null_When_Published_Date_Format_Incorrect()
        {
            server
                .Given(Request
                    .Create()
                    .WithPath("/v3/registration3/*")
                    .UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyFromFile(Path.GetFullPath(configuration["EndpointPackageMetadata:DateFormatIncorrect:Response"])));


            var publishedDate = await nugetApiService.GetPublishedDateByVersionAsync(
                configuration["EndpointPackageMetadata:DateFormatIncorrect:PackageName"],
                configuration["EndpointPackageMetadata:DateFormatIncorrect:Version"]);

            Assert.AreEqual(publishedDate, null);
        }

        [Test]
        public void GetPublishedDateByVersionAsync_Should_Throw_ArgumentNullException_When_Parameters_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await nugetApiService.GetPublishedDateByVersionAsync(null, ""));

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await nugetApiService.GetPublishedDateByVersionAsync("", null));
        }
    }
}