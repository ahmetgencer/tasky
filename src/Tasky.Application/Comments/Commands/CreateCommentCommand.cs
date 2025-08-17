using FluentValidation;
using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Application.Comments.DTOs;
using Tasky.Domain.Entities;

namespace Tasky.Application.Comments.Commands;

public sealed record CreateCommentCommand(Guid TaskId, string Content) : IRequest<CommentDto>;

public sealed class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
    }
}

public sealed class CreateCommentHandler(
    IRepository<Comment> repo,
    IRepository<TaskItem> taskRepo,
    ICurrentUser current
) : IRequestHandler<CreateCommentCommand, CommentDto>
{
    public async Task<CommentDto> Handle(CreateCommentCommand r, CancellationToken ct)
    {
        var uid = current.UserId ?? throw new ValidationException("Not authenticated");
        var task = await taskRepo.GetByIdAsync(r.TaskId, ct) ?? throw new ValidationException("Task not found");

        var entity = new Comment(task.Id, uid, r.Content);
        await repo.AddAsync(entity, ct);
        return CommentDto.FromEntity(entity);
    }
}
