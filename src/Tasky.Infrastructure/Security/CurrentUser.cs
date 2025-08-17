using System.Security.Claims;
using Tasky.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Tasky.Infrastructure.Security;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var http = accessor.HttpContext;
            var sub = http?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? http?.User?.FindFirst("sub")?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}
