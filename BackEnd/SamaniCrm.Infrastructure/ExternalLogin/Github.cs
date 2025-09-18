using SamaniCrm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.ExternalLogin;

public abstract class GitHub
{

    public class GitHubUser
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// github user name
        /// </summary>
        [JsonPropertyName("login")]
        public string Login { get; set; } = default!;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class GitHubEmail
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("primary")]
        public bool Primary { get; set; }

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }
    }

    public static async Task<(GitHubUser user, string email)> GetGitHubUserAsync(HttpClient _httpClient, string accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.UserAgent.ParseAdd(AppConsts.AppName);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<GitHubUser>(cancellationToken: cancellationToken);

        // اگر ایمیل مستقیم نبود → /user/emails
        if (string.IsNullOrEmpty(user!.Email))
        {
            using var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
            emailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            emailRequest.Headers.UserAgent.ParseAdd(AppConsts.AppName);

            var emailResponse = await _httpClient.SendAsync(emailRequest, cancellationToken);
            emailResponse.EnsureSuccessStatusCode();

            var emails = await emailResponse.Content.ReadFromJsonAsync<List<GitHubEmail>>(cancellationToken: cancellationToken);
            var primaryEmail = emails?.FirstOrDefault(e => e.Primary && e.Verified)?.Email ?? emails?.FirstOrDefault()?.Email;

            return (user, primaryEmail ?? "");
        }

        return (user, user.Email ?? "");
    }
}