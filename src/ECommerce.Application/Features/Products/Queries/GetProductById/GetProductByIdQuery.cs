using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Products
    .Queries.GetProductById;

public record GetProductByIdQuery(Guid Id)
    : IRequest<ProductDto>;
