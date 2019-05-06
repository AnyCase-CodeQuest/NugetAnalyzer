using System;
using Moq;
using NUnit.Framework;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class DirectoryServiceTests
    {
        private IDirectoryService directoryService;

        [SetUp]
        public void Init()
        {
            directoryService = new DirectoryService();
        }
    }
}
