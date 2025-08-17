using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasky.Application.Comments.Commands;
using Tasky.Application.Comments.DTOs;
using Tasky.Application.Comments.Queries;

namespace Tasky.Api.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public sealed class CommentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("tasks/{taskId:guid}/comments")]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetForTask(Guid taskId, CancellationToken ct)
    {
        var list = await mediator.Send(new GetCommentsForTaskQuery(taskId), ct);
        return Ok(list);
    }

    [HttpPost("tasks/{taskId:guid}/comments")]
    public async Task<ActionResult<CommentDto>> Create(Guid taskId, [FromBody] CreateCommentBody body, CancellationToken ct)
    {
        var dto = await mediator.Send(new CreateCommentCommand(taskId, body.Content), ct);
        return CreatedAtAction(nameof(GetForTask), new { taskId }, dto);
    }

    [HttpDelete("comments/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await mediator.Send(new DeleteCommentCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }

    public sealed record CreateCommentBody(string Content);
}
