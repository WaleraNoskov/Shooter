using Arch.Core;
using Arch.System;
using Microsoft.Xna.Framework;
using Shooter.Components;
using Shooter.Systems;

public class CollisionSystem : SystemBase<GameTime>
{
    // Все объекты с Position, Rigidbody и Collider
    private readonly QueryDescription _entities = new QueryDescription().WithAll<Position, Rigidbody, Collider>();

    public CollisionSystem(World world) : base(world)
    {
    }

    public override void Update(in GameTime state)
    {
        var entityList = new System.Collections.Generic.List<(Position pos, Rigidbody rb, Collider col)>();

        World.Query<Position, Rigidbody, Collider>(in _entities, (ref Position pos, ref Rigidbody rb, ref Collider col) =>
        {
            entityList.Add((pos, rb, col));
        });

        for (int i = 0; i < entityList.Count; i++)
        {
            var (posA, rbA, colA) = entityList[i];

            for (int j = i + 1; j < entityList.Count; j++)
            {
                var (posB, rbB, colB) = entityList[j];

                if (IsColliding(posA, colA, posB, colB))
                {
                    ResolveCollision(ref rbA, ref rbB);
                }
            }
        }

        for (int i = 0; i < entityList.Count; i++)
        {
            var (pos, rb, col) = entityList[i];
            World.Query<Position, Rigidbody, Collider>(in _entities, (ref Position p, ref Rigidbody r, ref Collider c) =>
            {
                if (ReferenceEquals(p, pos))
                {
                    r = rb;
                }
            });
        }
    }

    private static bool IsColliding(Position aPos, Collider aCol, Position bPos, Collider bCol)
    {
        double ax1 = aPos.Vector.X + aCol.X1;
        double ay1 = aPos.Vector.Y + aCol.Y1;
        double ax2 = aPos.Vector.X + aCol.X2;
        double ay2 = aPos.Vector.Y + aCol.Y2;

        double bx1 = bPos.Vector.X + bCol.X1;
        double by1 = bPos.Vector.Y + bCol.Y1;
        double bx2 = bPos.Vector.X + bCol.X2;
        double by2 = bPos.Vector.Y + bCol.Y2;

        return ax1 < bx2 && ax2 > bx1 && ay1 < by2 && ay2 > by1;
    }

    private static void ResolveCollision(ref Rigidbody rbA, ref Rigidbody rbB)
    {
        var newVelA = new Vector2(
            -rbA.Velocity.X + -rbA.Velocity.X * rbB.BouncingFactor,
            -rbA.Velocity.Y + -rbA.Velocity.Y * rbB.BouncingFactor
        );

        var newVelB = new Vector2(
            -rbB.Velocity.X + -rbB.Velocity.X * rbA.BouncingFactor,
            -rbB.Velocity.Y + -rbB.Velocity.Y * rbA.BouncingFactor
        );

        rbA.Velocity = newVelA;
        rbB.Velocity = newVelB;
    }
}
