using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Exceptions;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.DTOs;
using System.Net.Http.Json;

public class DummyJsonProductRepository : DummyJsonRepository<Product, DummyJsonProductListResponse>, IProductRepository
{
    public DummyJsonProductRepository(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override string ResourceUrl => "products";

    protected override IReadOnlyList<Product> ExtractList(DummyJsonProductListResponse wrapper)
    {
        return wrapper.Products.Select(p => new Product
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            Category = p.Category,
            Price = p.Price,
            Rating = p.Rating,
            Stock = p.Stock,
            Brand = p.Brand,
            Thumbnail = p.Thumbnail,
            Images = p.Images
        }).ToList();
    }

    public async Task<IReadOnlyList<Product>> FilterAsync(string? category, decimal? minPrice, decimal? maxPrice)
    {
        var products = await GetAllAsync();

        return products
            .Where(p => category is null || p.Category == category)
            .Where(p => minPrice is null || p.Price >= minPrice)
            .Where(p => maxPrice is null || p.Price <= maxPrice)
            .ToList();
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(string searchTerm)
    {
        var url = $"products/search?q={Uri.EscapeDataString(searchTerm)}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var wrapper = await response.Content.ReadFromJsonAsync<DummyJsonProductListResponse>();

        if (wrapper is null)
            throw new ExternalServiceUnavailableException(
                "External service returned empty response.",
                new Exception("Response body is null."));

        return ExtractList(wrapper);
    }
}