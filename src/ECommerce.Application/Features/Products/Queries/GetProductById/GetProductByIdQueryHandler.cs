using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler:IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id,cancellationToken);
        if (product == null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }
        // 3. Map to DTO
        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };

        // 4. Return DTO
        return dto;
    }
}
