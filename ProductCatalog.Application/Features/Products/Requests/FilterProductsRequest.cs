namespace ProductCatalog.Application.Features.Products.Requests
{
    public class FilterProductsRequest
    {
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
