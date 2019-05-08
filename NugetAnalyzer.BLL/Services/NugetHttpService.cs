using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Common;

namespace NugetAnalyzer.BLL.Services
{
    public class NugetHttpService : INugetHttpService
    {
        private readonly HttpClient client;
        private readonly NugetSettings nugetSettings;

        public NugetHttpService(HttpClient client, IOptions<NugetSettings> options)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            nugetSettings = options.Value;
        }

        public async Task<string> GetPackageMetadataAsync(string packageName, string version)
        {
            var url = $"{nugetSettings.PackageMetadata}/v3/registration3/{packageName.ToLowerInvariant()}/{version}.json";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetDataOfPackageVersionAsync(string query)
        {
            var url = $"{nugetSettings.Search}/query?q=PackageId:{WebUtility.UrlEncode(query ?? string.Empty)}&prerelease=false";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}