using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasky.Application.Projects.Commands;
using Tasky.Application.Projects.DTOs;
using Tasky.Application.Projects.Queries;

namespace Tasky.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateProjectCommand cmd)
    {
        var id = await mediator.Send(cmd);
        return CreatedAtAction(nameof(GetList), new { id }, new { id });
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetList([FromQuery] Guid? ownerId)
    {
        var list = await mediator.Send(new GetProjectsQuery(ownerId));
        return Ok(list);
    }
}
