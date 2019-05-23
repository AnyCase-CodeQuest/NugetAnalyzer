using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private INugetApiService nugetApiService;
        private Mock<INugetHttpService> nugetHttpServiceMock;
        private Mock<ILogger<NugetApiService>> loggerMock;

        [SetUp]
        public void Set()
        {
            nugetHttpServiceMock = new Mock<INugetHttpService>();
            loggerMock = new Mock<ILogger<NugetApiService>>();

            nugetApiService = new NugetApiService(nugetHttpServiceMock.Object, loggerMock.Object);
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Latest_Version_Of_Package_When_Package_Correct()
        {
            string packageName = "NUnit";
            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(await ResponsesReader.GetNUnitFromEndpointSearch());

            PackageVersion packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            Likeness<PackageVersion, PackageVersion> expectedVersion = new Likeness<PackageVersion, PackageVersion>(packageVersion);

            Assert.AreEqual(expectedVersion, new PackageVersion
            {
                Major = 3,
                Minor = 11,
                Build = 0,
                Revision = -1
            });

            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Package_Incorrect()
        {
            string packageName = "NonExistentPackage";
            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(await ResponsesReader.GetNotExistPackageFromEndpointSearch());

            PackageVersion packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            Assert.AreEqual(packageVersion, null);
            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Response_Not_Json()
        {
            string packageName = "NUnit";
            string response = string.Empty;

            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            PackageVersion packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            Assert.AreEqual(packageVersion, null);
            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Version_Of_Package_Incorrect()
        {
            string packageName = "NUnit";
            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(await ResponsesReader.GetIncorrectVersionFromEndpointSearch());

            PackageVersion packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            Assert.AreEqual(packageVersion, null);
            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(packageName));
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
            string packageName = "NUnit";
            string version = "3.9.0";
            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(await ResponsesReader.GetNUnitFromEndpointPackageMetadata());

            DateTime? publishedDate = await nugetApiService
                .GetPackagePublishedDateByVersionAsync(packageName, version);

            Assert.IsInstanceOf<DateTime?>(publishedDate);
            Assert.AreEqual(publishedDate, DateTime
                .Parse("2017-11-10T23:35:19+00:00")
                .ToUniversalTime());
            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(packageName, version));

        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_Null_When_Parameters_Incorrect()
        {
            string packageName = "NonExistentPackage";
            string version = "0.0.0";

            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(await ResponsesReader.GetNotFoundFromEndpointPackageMetadata());

            DateTime? publishedDate = await nugetApiService
                .GetPackagePublishedDateByVersionAsync(packageName, version);

            Assert.AreEqual(publishedDate, null);
            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(packageName, version));
        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_Null_When_Published_Date_Format_Incorrect()
        {
            string packageName = "NUnit";
            string version = "3.9.0";

            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(await ResponsesReader.GetDateFormatIncorrectFromEndpointPackageMetadata());

            DateTime? publishedDate = await nugetApiService
                .GetPackagePublishedDateByVersionAsync(packageName, version);

            Assert.AreEqual(publishedDate, null);
            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(packageName, version));
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