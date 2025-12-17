using System;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Shooter.Components;
using Shooter.Contracts;

namespace Shooter.Services.MovementHandlers;

public class VerticalPaddlerHandler : IMovementHandler
{
    public void Move(TimeSpan elapsedTime, ref Input input, ref Movement movement, EntityPhysicsObjects physicsObjects)
    {
        var joints = physicsObjects.GetAll<PrismaticJoint>(PhysicObjectTypes.PrismaticJoint);
        
        foreach (var joint in joints)
        {
            joint.MotorSpeed = movement.TargetVelocity * input.Y;
            joint.MaxMotorForce = movement.TargetForce;
        }
    }
}