using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;
using Shooter.EventComponents;

namespace Shooter.Systems;

public class PlayerCollisionSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _query = new QueryDescription().WithAll<Player, Collision>();

    public override void FixedUpdate(in float gameTime)
    {
        var collisionProcessing = new CollisionProcessing(world);
        World.InlineParallelQuery<CollisionProcessing, Player, Collision>(in _query, ref collisionProcessing);
    }

    private readonly struct CollisionProcessing(World world) : IForEach<Player, Collision>
    {
        public void Update(ref Player player, ref Collision collision)
        {
            player.Score++;
        }
    }
}