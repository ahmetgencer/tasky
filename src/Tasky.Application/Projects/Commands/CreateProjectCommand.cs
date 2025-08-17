using FluentValidation;
using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Domain.Entities;

namespace Tasky.Application.Projects.Commands;

public sealed record CreateProjectCommand(string Name) : IRequest<Guid>;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateProjectHandler(
    IRepository<Project> repo,
    ICurrentUser current
) : IRequestHandler<CreateProjectCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand r, CancellationToken ct)
    {
        var ownerId = current.UserId ?? throw new ValidationException("Not authenticated");
        var e = new Project(r.Name, ownerId);
        await repo.AddAsync(e, ct);
        return e.Id;
    }
}
