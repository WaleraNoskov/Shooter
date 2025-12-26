using Microsoft.Xna.Framework;

namespace Shooter.Components;

public struct ActualMovement
{
    public Vector2 Position;
    public Vector2 PreviousPosition;
    public float Angle;
    public float PreviousAngle;
}