using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ICacheService cacheService, ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Guid> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _unitOfWork
            .Repository<Category>()
            .GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
            throw new NotFoundException(
                nameof(Category), request.CategoryId);

        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Currency,
            request.StockQuantity,
            request.CategoryId,
            request.ImageUrl);

        await _unitOfWork
            .Repository<Product>()
            .AddAsync(product, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "🗑️ Invalidating cache — products:*");

        await _cacheService.RemoveByPatternAsync(
            "products:*", cancellationToken);

        _logger.LogInformation(
            "✅ Cache invalidated!");
        return product.Id;
    }
}
