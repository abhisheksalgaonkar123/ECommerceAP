using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery() : IRequest<List<CategoryDto>>;
