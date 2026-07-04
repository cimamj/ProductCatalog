using ProductCatalog.Domain.Exceptions;
using ProductCatalog.Domain.Interfaces;
using System.Net.Http.Json;
using System.Net;

public abstract class DummyJsonRepository<T, TWrapper> : IRepository<T>
    where T : class
    where TWrapper : class
{
    protected readonly HttpClient _httpClient;
    protected abstract string ResourceUrl { get; }

    protected DummyJsonRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync(ResourceUrl);
        response.EnsureSuccessStatusCode();

        var wrapper = await response.Content.ReadFromJsonAsync<TWrapper>();

        if (wrapper is null)
            throw new ExternalServiceUnavailableException(
                "External service returned empty response.",
                new Exception("Response body is null."));

        return ExtractList(wrapper);
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{ResourceUrl}/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected abstract IReadOnlyList<T> ExtractList(TWrapper wrapper);
}