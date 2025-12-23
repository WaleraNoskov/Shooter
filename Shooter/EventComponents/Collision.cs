using System.Numerics;
using Arch.Core;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Shooter.Contracts;

namespace Shooter.EventComponents;

public struct Collision
{
    public Vector2 Normal;
    public Vector2 Point;
    public CollisionLayer OtherLayer;
    public Vector2 OtherVelocity;
    public RectEdge OtherEdge;
    public float OtherEdgeLength;
}