﻿namespace Tasky.Application.Abstractions;

public interface ICurrentUser
{
    Guid? UserId { get; }
}
