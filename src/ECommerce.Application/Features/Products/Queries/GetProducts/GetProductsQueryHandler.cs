using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetProductsQueryHandler> _logger;
    // Cache key constant — consistent naming!
    private const string ProductsCacheKey = "products:all";

    public GetProductsQueryHandler(
        IUnitOfWork unitOfWork,
        ICacheService cacheService, ILogger<GetProductsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<List<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Check cache first — Cache-Aside pattern!
        var cachedProducts = await _cacheService
            .GetAsync<List<ProductDto>>(
                ProductsCacheKey,
                cancellationToken);

        if (cachedProducts is not null)
        {
            // Cache HIT — return immediately!
            _logger.LogInformation(
                "✅ CACHE HIT — returning {Count} products from Redis",
                cachedProducts.Count);
            return cachedProducts;
        }
        _logger.LogInformation("❌ CACHE MISS — querying database");
        // 2. Cache MISS — query database
        IReadOnlyList<Product> products;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm) ||
            request.CategoryId.HasValue ||
            request.IsActive.HasValue)
        {
            products = await _unitOfWork
                .Repository<Product>()
                .FindAsync(p =>
                    (string.IsNullOrWhiteSpace(request.SearchTerm) ||
                     p.Name.Contains(request.SearchTerm)) &&
                    (!request.CategoryId.HasValue ||
                     p.CategoryId == request.CategoryId.Value) &&
                    (!request.IsActive.HasValue ||
                     p.IsActive == request.IsActive.Value),
                    cancellationToken);
        }
        else
        {
            products = await _unitOfWork
                .Repository<Product>()
                .GetAllAsync(cancellationToken);
        }

        // 3. Get categories for mapping
        var categoryIds = products
            .Select(p => p.CategoryId)
            .Distinct()
            .ToList();

        var categories = await _unitOfWork
            .Repository<Category>()
            .FindAsync(c => categoryIds.Contains(c.Id),
                cancellationToken);

        var categoryLookup = categories
            .ToDictionary(c => c.Id, c => c.Name);

        // 4. Map to DTOs
        var productDtos = products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = categoryLookup
                .GetValueOrDefault(product.CategoryId, string.Empty),
            CreatedAt = product.CreatedAt,
            CreatedBy = product.CreatedBy
        }).ToList();

        // 5. Store in cache — 10 minute expiry
        await _cacheService.SetAsync(
            ProductsCacheKey,
            productDtos,
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return productDtos;
    }
}
