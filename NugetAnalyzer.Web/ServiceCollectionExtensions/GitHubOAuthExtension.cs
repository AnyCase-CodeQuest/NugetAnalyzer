using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NugetAnalyzer.Web.Options;

namespace NugetAnalyzer.Web.ServiceCollectionExtensions
{
    public static class GitHubOAuthExtension
    {
        private const string UserNameClaimType = "urn:github:login";
        private const string GithubUrlClaimType = "urn:github:url";
        private const string AvatarUrlClaimType = "urn:github:avatar";
        private const string GithubIdClaimType = ClaimTypes.NameIdentifier;

        public static void AddGitHubOAuth(this IServiceCollection services, IConfigurationSection endPointsSection, IConfigurationSection secretsSection)
        {
            services.Configure<GitHubSecretsOptions>(endPointsSection);
            services.Configure<GitHubEndPointsOptions>(secretsSection);

            var endPoints = endPointsSection.Get<GitHubEndPointsOptions>();
            var secrets = secretsSection.Get<GitHubSecretsOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
           .AddCookie()
           .AddOAuth("GitHub", options =>
           {
               options.ClientId = secrets.ClientId;
               options.ClientSecret = secrets.ClientSecret;
               options.CallbackPath = secrets.CallbackPath;

               options.AuthorizationEndpoint = endPoints.AuthorizationEndpoint;
               options.TokenEndpoint = endPoints.TokenEndpoint;
               options.UserInformationEndpoint = endPoints.UserInformationEndpoint;

               options.ClaimActions.MapJsonKey(GithubIdClaimType, "id");
               options.ClaimActions.MapJsonKey(UserNameClaimType, "login");
               options.ClaimActions.MapJsonKey(GithubUrlClaimType, "html_url");
               options.ClaimActions.MapJsonKey(AvatarUrlClaimType, "avatar_url");

               options.Scope.Add("repo");

               options.SaveTokens = true;

               options.Events = new OAuthEvents
               {
                   OnCreatingTicket = async context =>
                   {
                       var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                       request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                       request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                       var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                       response.EnsureSuccessStatusCode();
                       var jObjectResponse = await response.Content.ReadAsStringAsync();
                       var user = JObject.Parse(jObjectResponse);
                       context.RunClaimActions(user);
                   }
               };
           });
        }
    }
}
