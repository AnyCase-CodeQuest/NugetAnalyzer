using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NugetAnalyzer.Dtos.Converters;
using NugetAnalyzer.Web.Infrastructure.Models;
using NugetAnalyzer.Web.Infrastructure.Options;

namespace NugetAnalyzer.Web.Infrastructure.Extensions
{
	public static class ConverterExtensions
	{
		/// <summary>
		/// TODO: write comment
		/// </summary>
		/// <param name="services"></param>
		public static void AddConverters(this IServiceCollection services)
		{
			services.AddScoped<ProfileConverter>();
			services.AddScoped<UserConverter>();
		}

		/// <summary>
		/// TODO: write comment
		/// </summary>
		/// <param name="services"></param>
		/// <param name="gitHubEndPointsSection"></param>
		/// <param name="gitHubAppSettingsSection"></param>
		public static void AddGitHubOAuth(this IServiceCollection services, IConfigurationSection gitHubEndPointsSection, IConfigurationSection gitHubAppSettingsSection)
		{
			services.Configure<GitHubSecretsOptions>(gitHubEndPointsSection);
			services.Configure<GitHubEndPointsOptions>(gitHubAppSettingsSection);

			GitHubEndPointsOptions endPoints = gitHubEndPointsSection.Get<GitHubEndPointsOptions>();
			GitHubSecretsOptions secrets = gitHubAppSettingsSection.Get<GitHubSecretsOptions>();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = OAuthSourceNames.GitHubSourceName;
			})
			.AddCookie()
			.AddOAuth(OAuthSourceNames.GitHubSourceName, options =>
			{
				options.ClientId = secrets.ClientId;
				options.ClientSecret = secrets.ClientSecret;
				options.CallbackPath = secrets.CallbackPath;

				options.AuthorizationEndpoint = endPoints.AuthorizationEndpoint;
				options.TokenEndpoint = endPoints.TokenEndpoint;
				options.UserInformationEndpoint = endPoints.UserInformationEndpoint;

				options.ClaimActions.MapJsonKey(NugetAnalyzerClaimTypes.ExternalIdClaimType, GitHubUserClaimTypes.Id);
				options.ClaimActions.MapJsonKey(NugetAnalyzerClaimTypes.UserNameClaimType, GitHubUserClaimTypes.UserName);
				options.ClaimActions.MapJsonKey(NugetAnalyzerClaimTypes.GithubUrlClaimType, GitHubUserClaimTypes.Url);
				options.ClaimActions.MapJsonKey(NugetAnalyzerClaimTypes.AvatarUrlClaimType, GitHubUserClaimTypes.AvatarUrl);
				options.ClaimActions.MapJsonKey(NugetAnalyzerClaimTypes.SourceNameClaimType, NugetAnalyzerClaimTypes.SourceNameClaimType);

				options.Scope.Add(GitHubScopes.Repo);

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
						user.Add(NugetAnalyzerClaimTypes.SourceNameClaimType, OAuthSourceNames.GitHubSourceName);
						context.RunClaimActions(user);
					}
				};
			});
		}
	}
}
