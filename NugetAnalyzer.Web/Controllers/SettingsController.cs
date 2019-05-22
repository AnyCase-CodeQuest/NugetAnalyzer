using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NugetAnalyzer.Common.Configurations;

namespace NugetAnalyzer.Web.Controllers
{
    public class SettingsController : Controller
    {
        private readonly PackageVersionConfigurations packageVersionConfigurations;

        public SettingsController(IOptions<PackageVersionConfigurations> packageVersionConfigurations)
        {
            this.packageVersionConfigurations = (packageVersionConfigurations ?? throw new ArgumentNullException(nameof(packageVersionConfigurations))).Value;
        }
        public IActionResult Rules()
        {
            return View(packageVersionConfigurations);
        }
    }
}