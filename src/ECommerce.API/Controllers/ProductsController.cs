using ECommerce.Application.Features.Products.Commands.CreateProduct;
using ECommerce.Application.Features.Products.Queries.GetProductById;
using ECommerce.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1️⃣ GET api/products
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm,
            [FromQuery] Guid? categoryId,
            [FromQuery] bool? isActive)
        {
            var query = new GetProductsQuery(
                searchTerm,
                categoryId,
                isActive
            );

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // 2️⃣ GET api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetProductByIdQuery(id);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // 3️⃣ POST api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),          // links to GET by id
                new { id = result },      // route values
                result                   // response body
            );
        }
    }
}
