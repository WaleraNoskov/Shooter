using System;
using Shooter.Components;

namespace Shooter.Contracts;

public interface IMovementHandler
{
    void Move(float elapsedTime, ref TargetMovement targetMovement, EntityPhysicsObjects physicsObjects);
}