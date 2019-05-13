using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Domain;
using NUnit.Framework;
using SemanticComparison;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class NugetApiServiceTests
    {
        private IConfiguration configuration;
        private INugetApiService nugetApiService;
        private Mock<INugetHttpService> nugetHttpServiceMock;

        [OneTimeSetUp]
        public void Init()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(Environment.CurrentDirectory))
                .AddJsonFile("appsettings.json", false)
                .Build();
        }

        [SetUp]
        public void Set()
        {
            nugetHttpServiceMock = new Mock<INugetHttpService>();

            nugetApiService = new NugetApiService(nugetHttpServiceMock.Object);
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Latest_Version_Of_Package_When_Package_Correct()
        {
           var packageName = configuration["EndpointSearch:NUnit:PackageName"];
           var response = await File.ReadAllTextAsync(Path.GetFullPath(configuration["EndpointSearch:NUnit:Response"]));

           nugetHttpServiceMock
               .Setup(n => n.GetPackageMetadataAsync(It.IsAny<string>()))
               .ReturnsAsync(response);

            var packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            var expectedVersion = new Likeness<PackageVersion, PackageVersion>(packageVersion);

            Assert.AreEqual(expectedVersion, new PackageVersion
            {
                Major = 3,
                Minor = 11,
                Build = 0,
                Revision = -1
            });

            nugetHttpServiceMock.Verify(n => n.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Package_Incorrect()
        {
            var packageName = configuration["EndpointSearch:NotExist:PackageName"];
            var response = await File.ReadAllTextAsync(Path.GetFullPath(configuration["EndpointSearch:NotExist:Response"]));

            nugetHttpServiceMock
                .Setup(n => n.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            var packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            Assert.AreEqual(packageVersion, null);
            nugetHttpServiceMock.Verify(n => n.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Response_Not_Json()
        {
            var packageName = configuration["EndpointSearch:NUnit:PackageName"];
            var response = string.Empty;

            nugetHttpServiceMock
                .Setup(n => n.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            var packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            Assert.AreEqual(packageVersion, null);
            nugetHttpServiceMock.Verify(n => n.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Version_Of_Package_Incorrect()
        {
            var packageName = configuration["EndpointSearch:VersionIncorrect:PackageName"];
            var response = await File.ReadAllTextAsync(Path.GetFullPath(configuration["EndpointSearch:VersionIncorrect:Response"]));

            nugetHttpServiceMock
                .Setup(n => n.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            var packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            Assert.AreEqual(packageVersion, null);
            nugetHttpServiceMock.Verify(n => n.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public void GetLatestVersionPackageAsync_Should_Throw_ArgumentNullException_When_Package_Name_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await nugetApiService.GetLatestPackageVersionAsync(null));
        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_DateTime_When_Parameters_Correct()
        {
            var packageName = configuration["EndpointPackageMetadata:NUnit:Response"];
            var version = configuration["EndpointPackageMetadata:NUnit:Version"];

            var response = await File.ReadAllTextAsync(Path.GetFullPath(configuration["EndpointPackageMetadata:NUnit:Response"]));

            nugetHttpServiceMock
                .Setup(n => n.GetPackageVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            var publishedDate = await nugetApiService
                .GetPackagePublishedDateByVersionAsync(packageName, version);

            Assert.IsInstanceOf<DateTime?>(publishedDate);
            Assert.AreEqual(publishedDate, DateTime
                .Parse(configuration["EndpointPackageMetadata:NUnit:Published"])
                .ToUniversalTime());
            nugetHttpServiceMock.Verify(n => n.GetPackageVersionMetadataAsync(packageName, version));

        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_Null_When_Parameters_Incorrect()
        {
            var packageName = configuration["EndpointPackageMetadata:NotFound:Response"];
            var version = configuration["EndpointPackageMetadata:NotFound:Version"];

            var response = await File.ReadAllTextAsync(Path.GetFullPath(configuration["EndpointPackageMetadata:NotFound:Response"]));

            nugetHttpServiceMock
                .Setup(n => n.GetPackageVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            var publishedDate = await nugetApiService
                .GetPackagePublishedDateByVersionAsync(packageName, version);

            Assert.AreEqual(publishedDate, null);
            nugetHttpServiceMock.Verify(n => n.GetPackageVersionMetadataAsync(packageName, version));
        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_Null_When_Published_Date_Format_Incorrect()
        {
            var packageName = configuration["EndpointPackageMetadata:DateFormatIncorrect:Response"];
            var version = configuration["EndpointPackageMetadata:DateFormatIncorrect:Version"];

            var response = await File.ReadAllTextAsync(Path.GetFullPath(configuration["EndpointPackageMetadata:DateFormatIncorrect:Response"]));

            nugetHttpServiceMock
                .Setup(n => n.GetPackageVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            var publishedDate = await nugetApiService
                .GetPackagePublishedDateByVersionAsync(packageName, version);

            Assert.AreEqual(publishedDate, null);
            nugetHttpServiceMock.Verify(n => n.GetPackageVersionMetadataAsync(packageName, version));
        }

        [Test]
        public void GetPublishedDateByVersionAsync_Should_Throw_ArgumentNullException_When_Parameters_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await nugetApiService.GetPackagePublishedDateByVersionAsync(null, string.Empty));

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await nugetApiService.GetPackagePublishedDateByVersionAsync(string.Empty, null));
        }
    }
}