using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;



namespace BookTrackerAPI.Services
{
    public class HardcoverAuthService : IHealthCheck
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HardcoverAuthService(
            IMemoryCache cache,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration;

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_configuration["Hardcover:ApiUrl"]!);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var token = GetBearerToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BookBuddyApp/1.0");
        }

        public string GetApiUrl()
        {
            return _configuration["Hardcover:ApiUrl"]!;
        }

        public string GetApiKey()
        {
            return _configuration["Hardcover:ApiKey"]!;
        }

        public Task<string> GetAccessToken()
        {
            var apiKey = _configuration["Hardcover:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Hardcover API key is not configured.");

            return Task.FromResult(apiKey);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            const string healthQuery = "{ me { id } }";
            var content = new StringContent(
                $"{{\"query\":\"{healthQuery}\"}}",
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await _httpClient.PostAsync("", content, cancellationToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return HealthCheckResult.Unhealthy("Unauthorized – token expired or invalid");
                }

                response.EnsureSuccessStatusCode();
                return HealthCheckResult.Healthy("Hardcover API is reachable via GraphQL");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Hardcover API health check failed", ex);
            }
        }

        public string GetBearerToken()
        {
            return _configuration["Hardcover:BearerToken"]!;
        }

        private class TokenResponse
        {
            public string AccessToken { get; set; } = null!;
            public int ExpiresIn { get; set; }
            public string TokenType { get; set; } = null!;
        }
    }
}
