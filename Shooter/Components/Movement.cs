using System;
using Shooter.Contracts;

namespace Shooter.Components;

public struct Movement
{
    public MovementTypes Type;
    public float TargetVelocity;
    public float TargetForce;
}