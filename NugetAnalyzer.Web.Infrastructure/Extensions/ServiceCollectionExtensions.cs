using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NugetAnalyzer.Dtos.Converters;
using NugetAnalyzer.Web.Infrastructure.Configurations;

namespace NugetAnalyzer.Web.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers converters
        /// </summary>
        /// <param name="services"></param>
        public static void AddConverters(this IServiceCollection services)
        {
            services.AddScoped<ProfileConverter>();
            services.AddScoped<UserConverter>();
        }

        /// <summary>
        /// Registers GitHub OAuth as default Authentication
        /// </summary>
        /// <param name="services"></param>
        /// <param name="gitHubEndPointsSection">End points for githubAuthorization</param>
        /// <param name="gitHubAppSettingsSection">Settings from GitHub application</param>
        public static void AddGitHubOAuth(
            this IServiceCollection services,
            IConfigurationSection gitHubEndPointsSection,
            IConfigurationSection gitHubAppSettingsSection)
        {
            services.Configure<GitHubSecretsConfigurations>(gitHubEndPointsSection);
            services.Configure<GitHubEndPointsConfigurations>(gitHubAppSettingsSection);

            GitHubEndPointsConfigurations gitHubEndPointsConfigurations = gitHubEndPointsSection.Get<GitHubEndPointsConfigurations>();
            GitHubSecretsConfigurations gitHubSecretsConfigurations = gitHubAppSettingsSection.Get<GitHubSecretsConfigurations>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Constants.OAuthSourceNames.GitHubSourceName;
            })
            .AddCookie()
            .AddOAuth(Constants.OAuthSourceNames.GitHubSourceName, options =>
            {
                options.ClientId = gitHubSecretsConfigurations.ClientId;
                options.ClientSecret = gitHubSecretsConfigurations.ClientSecret;
                options.CallbackPath = gitHubSecretsConfigurations.CallbackPath;

                options.AuthorizationEndpoint = gitHubEndPointsConfigurations.AuthorizationEndpoint;
                options.TokenEndpoint = gitHubEndPointsConfigurations.TokenEndpoint;
                options.UserInformationEndpoint = gitHubEndPointsConfigurations.UserInformationEndpoint;

                options.ClaimActions.MapJsonKey(Constants.NugetAnalyzerClaimTypes.ExternalIdClaimType, Constants.GitHubUserClaimTypes.Id);
                options.ClaimActions.MapJsonKey(Constants.NugetAnalyzerClaimTypes.UserNameClaimType, Constants.GitHubUserClaimTypes.UserName);
                options.ClaimActions.MapJsonKey(Constants.NugetAnalyzerClaimTypes.GithubUrlClaimType, Constants.GitHubUserClaimTypes.Url);
                options.ClaimActions.MapJsonKey(Constants.NugetAnalyzerClaimTypes.AvatarUrlClaimType, Constants.GitHubUserClaimTypes.AvatarUrl);
                options.ClaimActions.MapJsonKey(Constants.NugetAnalyzerClaimTypes.SourceNameClaimType, Constants.NugetAnalyzerClaimTypes.SourceNameClaimType);

                options.Scope.Add(Constants.GitHubScopes.Repo);

                options.SaveTokens = true;

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        HttpResponseMessage response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();
                        string jObjectResponse = await response.Content.ReadAsStringAsync();
                        JObject user = JObject.Parse(jObjectResponse);
                        user.Add(Constants.NugetAnalyzerClaimTypes.SourceNameClaimType, Constants.OAuthSourceNames.GitHubSourceName);
                        context.RunClaimActions(user);
                    }
                };
            });
        }
    }
}
