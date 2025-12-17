using System;
using Microsoft.Xna.Framework;
using Shooter.Components;

namespace Shooter.Contracts;

public interface IMovementHandler
{
    void Move(TimeSpan elapsedTime, ref Input input, ref Movement movement, EntityPhysicsObjects physicsObjects);
}