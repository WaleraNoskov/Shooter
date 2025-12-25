using System.Collections.Generic;
using Shooter.Contracts;

namespace Shooter.Services;

public class MovementManager
{
    private readonly Dictionary<MovementTypes, IMovementHandler> _handlers = new();

    public void Register(MovementTypes type, IMovementHandler handler)
    {
        _handlers[type] = handler;
    }

    public IMovementHandler? Get(MovementTypes type) => _handlers.GetValueOrDefault(type);
}