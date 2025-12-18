using System;
using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Common;
using Shooter.Components;
using Shooter.EventComponents;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace Shooter.Systems;

public class CollisionProcessingSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _query = new QueryDescription().WithAll<CollisionEvent>();

    public override void Update(in GameTime gameTime)
    {
        var collisionProcessing = new CollisionProcessing(world);
        World.InlineParallelEntityQuery<CollisionProcessing, CollisionEvent>(in _query, ref collisionProcessing);
    }

    private readonly struct CollisionProcessing(World world) : IForEachWithEntity<CollisionEvent>
    {
        public void Update(Entity entity, ref CollisionEvent collisionEvent)
        {
            var foundBallInA = collisionEvent.EntityA.Has<Ball>();
            var foundBallInB = collisionEvent.EntityB.Has<Ball>();
            var foundPlayerInA = collisionEvent.EntityA.Has<Player>();
            var foundPlayerInB = collisionEvent.EntityB.Has<Player>();

            if (!foundBallInA && !foundBallInB || !foundPlayerInA && !foundPlayerInB)
            {
                world.Destroy(entity);
                return;
            }

            var ball = foundBallInA
                ? ref collisionEvent.EntityA.Get<Ball>()
                : ref collisionEvent.EntityB.Get<Ball>();
            var ballBody = foundBallInA
                ? collisionEvent.Contact.FixtureA.Body
                : collisionEvent.Contact.FixtureB.Body;

            var player = foundPlayerInA
                ? ref collisionEvent.EntityA.Get<Player>()
                : ref collisionEvent.EntityB.Get<Player>();
            var playerBody = foundPlayerInA
                ? collisionEvent.Contact.FixtureA.Body
                : collisionEvent.Contact.FixtureB.Body;
            var playerCollider = foundPlayerInA
                ? ref collisionEvent.EntityA.Get<RectangleCollider>()
                : ref collisionEvent.EntityB.Get<RectangleCollider>();

            collisionEvent.Contact.GetWorldManifold(out var normal, out var points);
            var contactPoint = points[0];

            var offset = contactPoint.Y - playerBody.Position.Y;
            var normalized = MathHelper.Clamp(offset / (playerCollider.Height / 2f) * -1, -1f, 1f);

            var bounceAngle = normalized * MathHelper.ToRadians(60f);
            float directionX = MathF.Sign(ballBody.LinearVelocity.X);
            
            var newVelocity = new Vector2(
                MathF.Cos(bounceAngle) * directionX * ball.TargetVelocity,
                MathF.Sin(bounceAngle) * ball.TargetVelocity
            );

            ballBody.LinearVelocity = newVelocity;

            world.Destroy(entity);
        }
    }
}