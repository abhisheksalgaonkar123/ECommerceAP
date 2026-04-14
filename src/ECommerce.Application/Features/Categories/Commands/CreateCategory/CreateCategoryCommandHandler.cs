using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler:IRequestHandler<CreateCategoryCommand,Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(
            request.Name,
            request.Description);
        await _unitOfWork.Repository<Category>().AddAsync(category,cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
