using System;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Shooter.Components;
using Shooter.Contracts;

namespace Shooter.Services.MovementHandlers;

public class VerticalPaddlerMovementHandler : IMovementHandler
{
    public void Move(TimeSpan elapsedTime, ref TargetMovement targetMovement, EntityPhysicsObjects physicsObjects)
    {
        var joints = physicsObjects.GetAll<PrismaticJoint>(PhysicObjectTypes.PrismaticJoint);
        
        foreach (var joint in joints)
        {
            joint.MotorSpeed = targetMovement.Velocity * targetMovement.Direction.Y;
            joint.MaxMotorForce = targetMovement.TargetForce;
        }
    }
}