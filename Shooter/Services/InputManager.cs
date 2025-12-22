using System.Collections.Generic;
using Shooter.Contracts;

namespace Shooter.Services;

public class InputManager
{
    private readonly Dictionary<MovementTypes, IInputHandler> _handlers = new();

    public void Register(MovementTypes type, IInputHandler handler)
    {
        _handlers[type] = handler;
    }

    public IInputHandler? Get(MovementTypes type) => _handlers.GetValueOrDefault(type);
}