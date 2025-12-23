using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Shooter.EventComponents;

namespace Shooter.Systems;

public class CollisionCleanupSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _query = new QueryDescription().WithAll<Collision>();

    public override void Update(in GameTime gameTime)
    {
        var cleanup = new CollisionCleanup(world);
        World.InlineParallelEntityQuery<CollisionCleanup, Collision>(in _query, ref cleanup);
    }

    private readonly struct CollisionCleanup(World world) : IForEachWithEntity<Collision>
    {
        public void Update(Entity entity, ref Collision collision)
        {
            entity.Remove<Collision>();
        }
    }
}