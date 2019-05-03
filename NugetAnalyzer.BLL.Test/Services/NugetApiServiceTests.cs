using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture]
    public class NugetApiServiceTests
    {
        private IConfiguration configuration;
        private INugetApiService nugetApiService;

        [OneTimeSetUp]
        public void Init()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Environment.CurrentDirectory, "../../../../NugetAnalyzer.Web"))
                .AddJsonFile("appsettings.json", false)
                .Build();

            nugetApiService = new NugetApiService(configuration);
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Latest_Version_Of_Package_When_Package_Correct()
        {
            var packageVersion = await nugetApiService.GetLatestVersionPackageAsync("NUnit");

            Assert.IsInstanceOf<PackageVersion>(packageVersion);
            Assert.GreaterOrEqual(packageVersion.Major, 3);
            Assert.GreaterOrEqual(packageVersion.Minor, 11);
            Assert.GreaterOrEqual(packageVersion.Build, 0);
            Assert.GreaterOrEqual(packageVersion.Revision, -1);
        }

        [Test]
        public async Task GetLatestVersionPackageAsync_Should_Return_Null_When_Package_Incorrect()
        {
            var packageVersion = await nugetApiService.GetLatestVersionPackageAsync("nonExistentPackage");

            Assert.AreEqual(packageVersion, null);
        }

        [Test]
        public void GetLatestVersionPackageAsync_Should_Throw_ArgumentNullException_When_Package_Name_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await nugetApiService.GetLatestVersionPackageAsync(null));
        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_DateTime_When_Parameters_Correct()
        {
            var publishedDate = await nugetApiService.GetPublishedDateByVersionAsync("NUnit", "3.9.0");

            Assert.IsInstanceOf<DateTime?>(publishedDate);
            Assert.AreEqual(publishedDate, new DateTime(2017,11,10,23,35,19));
        }

        [Test]
        public async Task GetPublishedDateByVersionAsync_Should_Return_Null_When_Parameters_Incorrect()
        {
            var publishedDate = await nugetApiService.GetPublishedDateByVersionAsync("NUnit", "0.0.0");

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