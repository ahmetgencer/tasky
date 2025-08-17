using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tasky.Application.Auth.Commands;
using Tasky.Application.Users.Commands;

namespace Tasky.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Register([FromBody] RegisterUserCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return Created(string.Empty, new { id });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Login([FromBody] LoginCommand cmd, CancellationToken ct)
    {
        var token = await mediator.Send(cmd, ct);
        return Ok(new { token });
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var user = HttpContext.User;

        var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? user.FindFirst("sub")?.Value;

        var email = user.FindFirst(ClaimTypes.Email)?.Value
                 ?? user.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        if (string.IsNullOrWhiteSpace(sub))
            return Unauthorized();

        return Ok(new { id = sub, email });
    }
}
