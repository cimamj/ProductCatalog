using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Features.Products.Requests;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Features.Products.Handlers
{
    public class FilterProductsHandler : RequestHandler<FilterProductsRequest, IReadOnlyList<ProductListItemDto>>
    {
        private readonly IProductRepository _productRepository;
        public FilterProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        protected override async Task<Result<IReadOnlyList<ProductListItemDto>>> HandleRequest(FilterProductsRequest request)
        {
            var products = await _productRepository.FilterAsync(request.Category, request.MinPrice, request.MaxPrice);

            var filteredProducts = products.Select(p => p.ToListItemDto()).ToList();

            return Result<IReadOnlyList<ProductListItemDto>>.Success(filteredProducts);
        }
    }
}
