using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Features.Products.Requests;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Interfaces;
using System.Collections.Generic;

namespace ProductCatalog.Application.Features.Products.Handlers
{
    public class GetAllProductsHandler : RequestHandler<GetAllProductsRequest, IReadOnlyList<ProductListItemDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        protected override async Task<Result<IReadOnlyList<ProductListItemDto>>> HandleRequest(GetAllProductsRequest request)
        {
            var products = await _productRepository.GetAllAsync();

            var productList = products.Select(p => p.ToListItemDto()).ToList();

            return Result<IReadOnlyList<ProductListItemDto>>.Success(productList);
        }
    }
}
