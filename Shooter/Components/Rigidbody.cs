using Microsoft.Xna.Framework;

namespace Shooter.Components;

public struct Rigidbody
{
    public Vector2 Velocity;
    public Vector2 MaxVelocity;
    public float BouncingFactor; //0 if static, 1 if fully bounces on collision
}