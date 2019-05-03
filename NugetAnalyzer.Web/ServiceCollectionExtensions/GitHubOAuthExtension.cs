using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace NugetAnalyzer.Web.ServiceCollectionExtensions
{
    public static class GitHubOAuthExtension
    {
        public static void AddGitHubOAuth(this IServiceCollection services, IConfigurationSection GitHubEndPoints, IConfigurationSection userSecrets)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
           .AddCookie()
           .AddOAuth("GitHub", options =>
           {
               options.ClientId = userSecrets.GetSection("ClientId").Value;
               options.ClientSecret = userSecrets.GetSection("ClientSecret").Value;
               options.CallbackPath = userSecrets.GetSection("CallbackPath").Value;

               options.AuthorizationEndpoint = GitHubEndPoints.GetSection("AuthorizationEndpoint").Value;
               options.TokenEndpoint = GitHubEndPoints.GetSection("TokenEndpoint").Value;
               options.UserInformationEndpoint = GitHubEndPoints.GetSection("UserInformationEndpoint").Value;

               options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
               options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
               options.ClaimActions.MapJsonKey("urn:github:login", "login");
               options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
               options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

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

                       var user = JObject.Parse(await response.Content.ReadAsStringAsync());
                       context.RunClaimActions(user);
                   }
               };
           });
        }
    }
}
