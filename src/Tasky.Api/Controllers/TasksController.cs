using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasky.Application.Tasks.Commands;
using Tasky.Application.Tasks.DTOs;
using Tasky.Application.Tasks.Queries;
using Tasky.Domain.Enums;

namespace Tasky.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateTaskCommand cmd)
    {
        var id = await mediator.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id)
    {
        var dto = await mediator.Send(new GetTaskByIdQuery(id));
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetList([FromQuery] Guid projectId, [FromQuery] TaskyStatus? status)
    {
        var list = await mediator.Send(new GetTasksQuery(projectId, status));
        return Ok(list);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskCommand body)
    {
        if (id != body.Id) return BadRequest("Route id and body id differ.");
        var ok = await mediator.Send(body);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await mediator.Send(new DeleteTaskCommand(id));
        return ok ? NoContent() : NotFound();
    }
}
