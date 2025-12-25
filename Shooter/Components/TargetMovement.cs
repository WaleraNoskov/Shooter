using System.Numerics;
using Shooter.Contracts;

namespace Shooter.Components;

public struct TargetMovement
{
    public MovementTypes Type;
    public float Velocity;
    public float MaxVelocity;
    public Vector2 Direction;
    public float TargetForce;
    public bool NeedToMove;
}