using System;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class NugetApiService : INugetApiService
    {
        private const string publishedDateFormat = "MM/dd/yyyy HH:mm:ss";
        private readonly INugetHttpService nugetHttpService;

        public NugetApiService(INugetHttpService nugetHttpService)
        {
            this.nugetHttpService = nugetHttpService ?? throw new ArgumentNullException(nameof(nugetHttpService));
        }

        public async Task<PackageVersion> GetLatestPackageVersionAsync(string packageName)
        {
            if (packageName == null)
            {
                throw new ArgumentNullException(nameof(packageName));
            }

            var data = await nugetHttpService.GetPackageMetadataAsync(packageName);

            return ParsePackageVersion(data);
        }

        public async Task<DateTime?> GetPackagePublishedDateByVersionAsync(string packageName, string version)
        {
            if (packageName == null)
            {
                throw new ArgumentNullException(nameof(packageName));
            }

            if (version == null)
            {
                throw new ArgumentNullException(nameof(packageName));
            }

            var data = await nugetHttpService.GetPackageVersionMetadataAsync(packageName, version);

            return ParsePublishedDate(data);
        }

        private PackageVersion ParsePackageVersion(string response)
        {
            JObject jsonObject;
            try
            {
                jsonObject = JObject.Parse(response);
            }
            catch (JsonReaderException)
            {
                return null;
            }

            var totalHitsString = (string)jsonObject["totalHits"];

            if (!int.TryParse(totalHitsString, out var totalHits) || totalHits == 0)
            {
                return null;
            }

            var data = jsonObject["data"];

            var versionString = (string)data[0]["version"];
            if (Version.TryParse(versionString, out var version))
            {
                return new PackageVersion
                {
                    Major = version.Major,
                    Minor = version.Minor,
                    Build = version.Build,
                    Revision = version.Revision
                };
            }

            return null;
        }

        private DateTime? ParsePublishedDate(string response)
        {
            JObject jsonObject;
            try
            {
                jsonObject = JObject.Parse(response);
            }
            catch (JsonReaderException)
            {
                return null;
            }

            var publishedString = (string)jsonObject["published"];
            if (DateTime.TryParseExact(
                            publishedString,
                            publishedDateFormat,
                            null,
                            DateTimeStyles.AdjustToUniversal 
                            | DateTimeStyles.AssumeLocal,
                            out var published))
            {
                return published;
            }

            return null;
        }
    }
}
