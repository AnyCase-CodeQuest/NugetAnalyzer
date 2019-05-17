using System.Threading.Tasks;
using System.IO;

namespace NugetAnalyzer.BLL.Test
{
    public static class ResponsesReader
    {
        private const string PathToEndpointPackageMetadata = "Responses/EndpointPackageMetadata";
        private const string PathToEndpointSearch = "Responses/EndpointSearch";

        public static Task<string> GetNUnitFromEndpointPackageMetadata()
        {
            string path = Path.Combine(PathToEndpointPackageMetadata, "NUnitResponse.json");
            return GetContent(path);
        }

        public static Task<string> GetNotFoundFromEndpointPackageMetadata()
        {
            string path = Path.Combine(PathToEndpointPackageMetadata, "NotFoundResponse.xml");
            return GetContent(path);
        }

        public static Task<string> GetDateFormatIncorrectFromEndpointPackageMetadata()
        {
            string path = Path.Combine(PathToEndpointPackageMetadata, "DateFormatIncorrectResponse.json");
            return GetContent(path);
        }

        public static Task<string> GetNUnitFromEndpointSearch()
        {
            string path = Path.Combine(PathToEndpointSearch, "NUnitResponse.json");
            return GetContent(path);
        }

        public static Task<string> GetIncorrectVersionFromEndpointSearch()
        {
            string path = Path.Combine(PathToEndpointSearch, "IncorrectVersionResponse.json");
            return GetContent(path);
        }

        public static Task<string> GetNotExistPackageFromEndpointSearch()
        {
            string path = Path.Combine(PathToEndpointSearch, "NotExistPackageResponse.json");
            return GetContent(path);
        }

        private static Task<string> GetContent(string path)
        {
            StreamReader streamReader = new StreamReader(path, System.Text.Encoding.Default);
            return streamReader.ReadToEndAsync();
        }
    }
}