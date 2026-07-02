namespace ProductCatalog.Application.DTOs
{
    public class ProductListItemDto
    {
        //AI assisted : Claude was used to determine relevant DTO fields for list item based on typical product catalog
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
    }
}
