using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Exceptions;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.DTOs;
using System.Net.Http.Json;
using System.Net;

public class DummyJsonAuthRepository : IAuthRepository
{
    private readonly HttpClient _httpClient;

    public DummyJsonAuthRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetTokenAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login", new
        {
            username,
            password,
            expiresInMins = 30
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return null;

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DummyJsonLoginResponse>();
        return result?.AccessToken;
    }

    public async Task<User?> GetCurrentUserAsync(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "auth/me");
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceUnavailableException(
                "Could not retrieve user from external service.",
                new Exception($"Status code: {response.StatusCode}"));

        var dto = await response.Content.ReadFromJsonAsync<DummyJsonUserDto>();

        return new User
        {
            Id = dto!.Id,
            Username = dto.Username,
            Role = dto.Role
        };
    }
}