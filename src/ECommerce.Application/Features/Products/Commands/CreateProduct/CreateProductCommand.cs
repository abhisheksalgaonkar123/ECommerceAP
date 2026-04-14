using MediatR;

namespace ECommerce.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name,
string Description,
decimal Price,
string Currency,
int StockQuantity,
Guid CategoryId,
string? ImageUrl): IRequest<Guid>;
