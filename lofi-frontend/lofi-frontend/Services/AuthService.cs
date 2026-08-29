using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using lofi_frontend.Models;

namespace lofi_frontend.Services;

public class AuthService
{
    private readonly AccessTokenService _ats;
    private readonly NavigationManager _nav;
    private readonly HttpClient _client;


    public AuthService(
        AccessTokenService accessTokenService,
        NavigationManager nav,
        IHttpClientFactory httpClientFactory
        )
    {
        _ats =  accessTokenService;
        _nav = nav;
        _client = httpClientFactory.CreateClient("BackendApi");
    }

    public async Task<bool> Login(string email, string password)
    {
        Console.WriteLine("Making login request");
        var status = await _client.PostAsJsonAsync(
            "Auth/sign-in", new { email, password });
        if (!status.IsSuccessStatusCode) return false;
        
        Console.WriteLine($"Login successful: {await status.Content.ReadAsStringAsync()}");
        
        var token = await status.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (token is null) return false;
        Console.WriteLine($"Token: {token["accessToken"]}");

        if (!string.IsNullOrWhiteSpace(token["accessToken"])) 
            await _ats.SetToken(token["accessToken"]);
        else return false;
        Console.WriteLine("Token set");
        return true;
    }

    public async Task<bool> SignUp(UserWithPassword user)
    {
        Console.WriteLine("Signing up");
        var response = await _client.PostAsJsonAsync(
            "Auth/sign-up", new { user.UserData.Email, user.Password });
        if (response.IsSuccessStatusCode)
        {
            await Login(user.UserData.Email, user.Password);
            var rawJwt = await _ats.GetToken();
            var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(rawJwt);
            var subClaim = readJwt.Claims.FirstOrDefault(c => c.Type == "sub");
            user.UserData.Id = subClaim?.Value;

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", rawJwt);

            var createResponse = await _client.PostAsJsonAsync("Users", user);

            return createResponse.IsSuccessStatusCode;

        }

        return false;
    }

    public async Task<UserData?> GetCurrentUser()
    {
        var token = await _ats.GetToken();
        if (string.IsNullOrWhiteSpace(token)) return null;

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("Users/me");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<UserData>();
    }

    public async Task Logout()
    {
        var token = await _ats.GetToken();
        if (!string.IsNullOrWhiteSpace(token))
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                await _client.PostAsync("Auth/sign-out", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling sign-out: {ex.Message}");
            }
        }

        await _ats.RemoveToken();
    }
}

public class AuthResponse
{
    public string AccessToken { get; init; } = "";
    
    public string RefreshToken { get; } = "";
}
