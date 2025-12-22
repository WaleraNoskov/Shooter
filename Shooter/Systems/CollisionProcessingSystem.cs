using System;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Shooter.Components;
using Shooter.Contracts;
using Shooter.EventComponents;
using Shooter.Services;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;
using World = Arch.Core.World;

namespace Shooter.Systems;

public class CollisionProcessingSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _query = new QueryDescription().WithAll<CollisionEvent>();

    public override void Update(in GameTime gameTime)
    {
        var collisionProcessing = new CollisionProcessing(world);
        World.InlineParallelEntityQuery<CollisionProcessing, CollisionEvent>(in _query, ref collisionProcessing);
    }

    private readonly struct CollisionProcessing(World world)
        : IForEachWithEntity<CollisionEvent>
    {
        public void Update(Entity entity, ref CollisionEvent collisionEvent)
        {
            var foundBallInA = collisionEvent.EntityA.Has<Ball, TargetMovement, ActualMovement>();
            var foundBallInB = collisionEvent.EntityB.Has<Ball, TargetMovement, ActualMovement>();
            var foundPlayerInA = collisionEvent.EntityA.Has<Player, ActualMovement>();
            var foundPlayerInB = collisionEvent.EntityB.Has<Player, ActualMovement>();

            if (!foundBallInA && !foundBallInB || !foundPlayerInA && !foundPlayerInB)
            {
                world.Destroy(entity);
                return;
            }

            var ballActualMovement = foundBallInA
                ? ref collisionEvent.EntityA.Get<ActualMovement>()
                : ref collisionEvent.EntityB.Get<ActualMovement>();
            var ballTargetMovement = foundBallInA
                ? ref collisionEvent.EntityA.Get<TargetMovement>()
                : ref collisionEvent.EntityB.Get<TargetMovement>();

            var player = foundPlayerInA
                ? ref collisionEvent.EntityA.Get<Player>()
                : ref collisionEvent.EntityB.Get<Player>();
            var playerActualMovement = foundPlayerInA
                ? ref collisionEvent.EntityA.Get<ActualMovement>()
                : ref collisionEvent.EntityB.Get<ActualMovement>();
            var playerCollider = foundPlayerInA
                ? ref collisionEvent.EntityA.Get<RectangleCollider>()
                : ref collisionEvent.EntityB.Get<RectangleCollider>();
            
            player.Score++;

            var ballPos = ballActualMovement.Position;
            var paddlePos = playerActualMovement.Position;

            var offsetY = ballPos.Y - paddlePos.Y;
            var halfHeight = playerCollider.Height * 0.5f;
            var normalized = Math.Clamp(offsetY / halfHeight, -1f, 1f);

            const float maxBounceAngle = MathF.PI / 3f;
            var angle = normalized * maxBounceAngle;

            var directionX = ballPos.X < paddlePos.X ? -1f : 1f;

            System.Numerics.Vector2 newDirection = new(
                MathF.Cos(angle) * directionX,
                MathF.Sin(angle)
            );
            newDirection = System.Numerics.Vector2.Normalize(newDirection);

            ballTargetMovement.Direction = new System.Numerics.Vector2(newDirection.X, newDirection.Y);

            ballTargetMovement.NeedToMove = true;
            
            world.Destroy(entity);
        }
    }
}