using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.Web.Controllers
{
    public class PackageController : Controller
    {
        public IActionResult ListPackages()
        {
           var model = new ProjectViewModel
           {
               ProjectName = "NugetAnalyzer.Web",
               Packages = GetListPackages()
           };

            return View(model);
        }

        

        public List<PackageViewModel> GetListPackages()
        {
            var package1 = new PackageViewModel
            {
                Name = "Newtonsoft.Json",
                Id = 1,
                Current = new PackageVersion
                    {
                        Id = 1,
                        Major = 1,
                        Minor = 2,
                        Build = 3,
                        Revision = -1,
                        PackageId = 1,
                        PublishedDate = DateTime.Now
                    },
                Latest = new PackageVersion
                    {
                        Id = 2,
                        Major = 2,
                        Minor = 3,
                        Build = 3,
                        Revision = -1,
                        PackageId = 1,
                        PublishedDate = DateTime.Now
                    }
                
            };
            var package2 = new PackageViewModel
            {
                Name = "EntityFramework",
                Id = 2,
                LastUpdateTime = DateTime.Now,
                Current = new PackageVersion
                    {
                        Id = 3,
                        Major = 5,
                        Minor = 2,
                        Build = 3,
                        Revision = -1,
                        PackageId = 1,
                        PublishedDate = DateTime.Now
                    },
                  Latest = new PackageVersion
                    {
                        Id = 4,
                        Major = 6,
                        Minor = 3,
                        Build = 3,
                        Revision = -1,
                        PackageId = 1,
                        PublishedDate = DateTime.Now
                    }
            };

            return new List<PackageViewModel>
            {
                package2,
                package1
            };

        }
    }

    public class ProjectViewModel
    {
        public string ProjectName { get; set; }
        public IList<PackageViewModel> Packages { get; set; }
    }
     
    public class PackageViewModel
    {
        public int Id { get; set; }
        public DateTime LastUpdateTime { get; set; } = DateTime.Now;
        public string Name { get; set; }
        public PackageVersion Current { get; set; }
        public PackageVersion Latest { get; set; }
        public string Report { get; set; }
    }
}