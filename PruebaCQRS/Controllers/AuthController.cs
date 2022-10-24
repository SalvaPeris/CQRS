using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaCQRS.Features.Auth.Command;
using PruebaCQRS.Services;
using System.Runtime.Serialization;

namespace PruebaCQRS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost]
        public Task<TokenCommandResponse> Token([FromBody] TokenCommand command) => _mediator.Send(command);

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me([FromServices] ICurrentUserService currentUserService)
        {
            return Ok(new
            {
                currentUserService.User,
                IsAdmin = currentUserService.IsInRole("Admin")
            });
        }
    }
}
