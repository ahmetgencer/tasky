using FluentValidation;
using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Domain.Entities;

namespace Tasky.Application.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<string>;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class LoginHandler(
    IRepository<User> repo,
    IPasswordHasher hasher,
    IJwtProvider jwt
) : IRequestHandler<LoginCommand, string>
{
    public async Task<string> Handle(LoginCommand r, CancellationToken ct)
    {
        var users = await repo.ListAsync(u => u.Email == r.Email, ct);
        var user = users[0] ?? throw new ValidationException("Invalid credentials");

        if (!hasher.Verify(user.PasswordHash, r.Password))
            throw new ValidationException("Invalid credentials");

        return jwt.Generate(user);
    }
}
