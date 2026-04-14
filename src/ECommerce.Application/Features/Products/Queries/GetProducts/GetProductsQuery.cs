using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Products
    .Queries.GetProducts;

public record GetProductsQuery(
    string? SearchTerm,
    Guid? CategoryId,
    bool? IsActive)
    : IRequest<List<ProductDto>>;
