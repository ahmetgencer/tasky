using Tasky.Domain.Entities;

namespace Tasky.Application.Abstractions;

public interface IJwtProvider
{
    string Generate(User user);
}
