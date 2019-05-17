using System;
using System.Threading.Tasks;
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

        [SetUp]
        public void Set()
        {
            nugetHttpServiceMock = new Mock<INugetHttpService>();

            nugetApiService = new NugetApiService(nugetHttpServiceMock.Object);
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Latest_Version_Of_Package_When_Package_Correct()
        {
            var packageName = "NUnit";
            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(await ResponseReader.GetNUnitFromEndpointSearch());

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

            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Package_Incorrect()
        {
            var packageName = "NonExistentPackage";
            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(await ResponseReader.GetNotExistPackageFromEndpointSearch());

            var packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            Assert.AreEqual(packageVersion, null);
            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Response_Not_Json()
        {
            var packageName = "NUnit";
            var response = string.Empty;

            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            var packageVersion = await nugetApiService
                .GetLatestPackageVersionAsync(packageName);

            Assert.AreEqual(packageVersion, null);
            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(packageName));
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Version_Of_Package_Incorrect()
        {
            var packageName = "NUnit";
            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageMetadataAsync(It.IsAny<string>()))
                .ReturnsAsync(await ResponseReader.GetIncorrectVersionFromEndpointSearch());

            var packageVersion = await nugetApiService
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
            var packageName = "NUnit";
            var version = "3.9.0";
            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(await ResponseReader.GetNUnitFromEndpointPackageMetadata());

            var publishedDate = await nugetApiService
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
            var packageName = "NonExistentPackage";
            var version = "0.0.0";

            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(await ResponseReader.GetNotFoundFromEndpointPackageMetadata());

            var publishedDate = await nugetApiService
                .GetPackagePublishedDateByVersionAsync(packageName, version);

            Assert.AreEqual(publishedDate, null);
            nugetHttpServiceMock.Verify(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(packageName, version));
        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_Null_When_Published_Date_Format_Incorrect()
        {
            var packageName = "NUnit";
            var version = "3.9.0";

            nugetHttpServiceMock
                .Setup(nugetHttpService => nugetHttpService.GetPackageVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(await ResponseReader.GetDateFormatIncorrectFromEndpointPackageMetadata());

            var publishedDate = await nugetApiService
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