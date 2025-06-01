using Microsoft.Extensions.Configuration;
using SRPM.API.Models;
using System.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SRPM.API.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(IConfiguration configuration, HttpClient httpClient, IAuthService authService, ILogger<GoogleAuthService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
        }

        public string GetGoogleAuthUrl(string action)
        {
            var clientId = _configuration["Google:ClientId"];
            var scope = "openid email profile";
            var redirectUri = action == "signin"
                ? _configuration["Google:SigninRedirectUri"]
                : _configuration["Google:SignupRedirectUri"];

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                        $"?client_id={clientId}" +
                        $"&redirect_uri={HttpUtility.UrlEncode(redirectUri)}" +
                        $"&scope={HttpUtility.UrlEncode(scope)}" +
                        $"&response_type=code" +
                        $"&state={action}";

            _logger.LogInformation("Generated Google Auth URL for action: {Action}, Redirect URI: {RedirectUri}", action, redirectUri);
            return authUrl;
        }

        public async Task<AuthResponse> HandleGoogleCallbackAsync(string code, string action)
        {
            try
            {
                _logger.LogInformation("Handling Google callback for action: {Action}", action);
                
                // Exchange code for token
                var tokenResponse = await ExchangeCodeForTokenAsync(code, action);
                if (tokenResponse == null)
                {
                    _logger.LogError("Failed to exchange code for token");
                    return new AuthResponse { Success = false, Message = "Failed to exchange code for token" };
                }

                // Get user info from Google
                var userInfo = await GetGoogleUserInfoAsync(tokenResponse.AccessToken);
                if (userInfo == null)
                {
                    _logger.LogError("Failed to get user info from Google");
                    return new AuthResponse { Success = false, Message = "Failed to get user info from Google" };
                }

                // Create Google login request
                var googleLoginRequest = new GoogleLoginRequest
                {
                    GoogleToken = tokenResponse.IdToken
                };

                // Use existing Google login logic
                return await _authService.LoginWithGoogleAsync(googleLoginRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google authentication failed");
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Google authentication failed", 
                    Error = ex.Message 
                };
            }
        }

        private async Task<GoogleTokenResponse?> ExchangeCodeForTokenAsync(string code, string action)
        {
            var clientId = _configuration["Google:ClientId"];
            var clientSecret = _configuration["Google:ClientSecret"];
            var redirectUri = action == "signin"
                ? _configuration["Google:SigninRedirectUri"]
                : _configuration["Google:SignupRedirectUri"];

            _logger.LogInformation("Exchanging code for token with redirect URI: {RedirectUri}", redirectUri);

            var tokenRequest = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"redirect_uri", redirectUri},
                {"grant_type", "authorization_code"}
            };

            var content = new FormUrlEncodedContent(tokenRequest);
            
            try
            {
                var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Token exchange response status: {StatusCode}", response.StatusCode);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Token exchange failed. Status: {StatusCode}, Response: {Response}", 
                        response.StatusCode, responseContent);
                    return null;
                }

                var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(responseContent, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Successfully exchanged code for token");
                return tokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during token exchange");
                return null;
            }
        }

        private async Task<GoogleUserInfo?> GetGoogleUserInfoAsync(string accessToken)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get user info. Status: {StatusCode}, Response: {Response}", 
                        response.StatusCode, responseContent);
                    return null;
                }

                var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(responseContent, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Successfully retrieved user info for email: {Email}", userInfo?.Email);
                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting user info");
                return null;
            }
        }

        private class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;
            
            [JsonPropertyName("id_token")]
            public string IdToken { get; set; } = string.Empty;
            
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; } = string.Empty;
            
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
            
            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }
            
            [JsonPropertyName("scope")]
            public string? Scope { get; set; }
        }

        private class GoogleUserInfo
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
            
            [JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;
            
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            
            [JsonPropertyName("picture")]
            public string Picture { get; set; } = string.Empty;
            
            [JsonPropertyName("given_name")]
            public string? GivenName { get; set; }
            
            [JsonPropertyName("family_name")]
            public string? FamilyName { get; set; }
            
            [JsonPropertyName("verified_email")]
            public bool VerifiedEmail { get; set; }
        }
    }
}