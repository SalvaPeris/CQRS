using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Features.Products.Queries;
using ApplicationCore.Features.Products.Commands;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public ProductsController (IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{ProductId}")]
        public Task<GetProductQueryResponse> GetProductById([FromRoute] GetProductQuery query) => _mediator.Send(query);

        [HttpGet]
        public Task<List<GetProductsQueryResponse>> GetProducts() => _mediator.Send(new GetProductsQuery());

        // POST api/<ProductsController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public void Put()
        {
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{ProductId}")]
        public async void DeleteProduct([FromRoute] DeleteProductCommand command)
        {
            await _mediator.Send(command);
        }
    }
}
