using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class NugetApiService : INugetApiService
    {
        private const string publishedDateFormat = "MM/dd/yyyy HH:mm:ss";
        private readonly IConfiguration configuration;

        public NugetApiService(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private async Task<string> GetPackageMetadataAsync(string packageName, string version)
        {
            var url = $"{configuration["EndpointsNugetApi:PackageMetadata"]}/v3/registration3/{packageName.ToLowerInvariant()}/{version}.json";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return  await response.Content.ReadAsStringAsync();
            }
        }

        private async Task<string> GetDataOfPackageVersionAsync(string query)
        {
            var url = $"{configuration["EndpointsNugetApi:Search"]}/query?q=PackageId:{WebUtility.UrlEncode(query ?? string.Empty)}&prerelease=false";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
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

        public async Task<PackageVersion> GetLatestVersionPackageAsync(string packageName)
        {
            if (packageName == null)
            {
                throw new ArgumentNullException(nameof(packageName));
            }

            var data = await GetDataOfPackageVersionAsync(packageName);

            return ParsePackageVersion(data);
        }

        public async Task<DateTime?> GetPublishedDateByVersionAsync(string packageName, string version)
        {
            if (packageName == null)
            {
                throw new ArgumentNullException(nameof(packageName));
            }

            if (version == null)
            {
                throw new ArgumentNullException(nameof(packageName));
            }

            var data = await GetPackageMetadataAsync(packageName, version);

            return ParsePublishedDate(data);
        }
    }
}
