using System;
using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;
using Shooter.Contracts;
using Shooter.EventComponents;
using Vector2 = System.Numerics.Vector2;

namespace Shooter.Systems;

public class BallCollisionSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _query = new QueryDescription().WithAll<Ball, Collision, TargetMovement>();

    public override void Update(in GameTime gameTime)
    {
        var collisionProcessing = new CollisionProcessing(world);
        World.InlineParallelQuery<CollisionProcessing, Ball, Collision, TargetMovement, ActualMovement>(in _query, ref collisionProcessing);
    }

    private readonly struct CollisionProcessing(World world) : IForEach<Ball, Collision, TargetMovement, ActualMovement>
    {
        public void Update(ref Ball ball, ref Collision collision, ref TargetMovement targetMovement, ref ActualMovement actualMovement)
        {
            switch (collision.OtherLayer)
            {
                case CollisionLayer.Player:
                    BounceFromPaddle(ref collision, ref targetMovement, ref actualMovement);
                    break;
                case CollisionLayer.Wall:
                    BounceFromWall(ref collision, ref targetMovement);
                    break;
            }
        }
    }

    private static void BounceFromPaddle(
        ref Collision collision,
        ref TargetMovement movement,
        ref ActualMovement actual)
    {
        const float maxBounceAngle = MathF.PI / 3f; 
        
        var halfEdge = collision.OtherEdgeLength * 0.5f;
        var offset = collision.OtherEdge is RectEdge.Left or RectEdge.Right
                ? actual.Position.Y - collision.Point.Y
                : actual.Position.X - collision.Point.X;
        
        var normalized = Math.Clamp(offset / halfEdge, -1f, 1f);
        var angle = normalized * maxBounceAngle;
        var directionX = MathF.Sign(collision.Normal.X);
        var dir = new Vector2(
            MathF.Cos(angle) * directionX,
            MathF.Sin(angle)
        );

        dir += collision.OtherVelocity * 0.02f;

        movement.Direction = Vector2.Normalize(dir);
        movement.NeedToMove = true;
    }

    private static void BounceFromWall(
        ref Collision collision,
        ref TargetMovement movement)
    {
        movement.Direction = Vector2.Reflect(
            movement.Direction,
            collision.Normal
        );

        movement.NeedToMove = true;
    }
}