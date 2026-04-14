using MediatR;

namespace ECommerce.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description):IRequest<Guid>;
