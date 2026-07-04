namespace ProductCatalog.Infrastructure.DTOs
{
    // AI assisted: Claude analyzed DummyJSON API response structure and determined required wrapper class fields
    public class DummyJsonProductListResponse
    {
        public List<DummyJsonProductDto> Products { get; set; } = new();
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}
