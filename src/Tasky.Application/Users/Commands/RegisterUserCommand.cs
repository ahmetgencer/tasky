using FluentValidation;
using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Domain.Entities;

namespace Tasky.Application.Users.Commands;

public sealed record RegisterUserCommand(string Email, string Password) : IRequest<Guid>;

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public sealed class RegisterUserHandler(
    IRepository<User> repo,
    IPasswordHasher hasher
) : IRequestHandler<RegisterUserCommand, Guid>
{
    public async Task<Guid> Handle(RegisterUserCommand r, CancellationToken ct)
    {
        var existing = await repo.ListAsync(u => u.Email == r.Email, ct);
        if (existing.Count > 0)
            throw new ValidationException($"Email already exists: {r.Email}");

        var hash = hasher.Hash(r.Password);
        var user = new User(r.Email, hash);
        await repo.AddAsync(user, ct);
        return user.Id;
    }
}
