using System;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Shooter.Components;
using Shooter.Contracts;

namespace Shooter.Services.MovementHandlers;

public class BallMovementHandler : IMovementHandler
{
    public void Move(TimeSpan elapsedTime, ref TargetMovement targetMovement, EntityPhysicsObjects physicsObjects)
    {
        var body = physicsObjects.GetByTag<Body>(PhysicObjectTypes.PhysicsBody, PhysicTags.MainBody);
        if (body is null)
            return;

        var newVelocity = new Vector2(targetMovement.Direction.X, targetMovement.Direction.Y)
                          * targetMovement.Velocity;
        body.LinearVelocity = newVelocity;
    }
}