using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;
using Shooter.Contracts;
using Shooter.EventComponents;
using Shooter.Services;

namespace Shooter.Systems;

public class LooseCatcherCollisionSystem(World world, GameManager gameManager) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _query = new QueryDescription().WithAll<Collision, LooseCatcher>();

    public override void FixedUpdate(in float gameTime)
    {
        var collisionProcessing = new CollisionProcessing(world, gameManager);
        World.InlineParallelQuery<CollisionProcessing, Collision, LooseCatcher>(in _query, ref collisionProcessing);
    }

    private readonly struct CollisionProcessing(World world, GameManager gameManager) : IForEach<Collision, LooseCatcher>
    {
        public void Update(ref Collision collision, ref LooseCatcher looseCatcher)
        {
            gameManager.WinnerPlayerIndex = looseCatcher.PlayerIndex == 1 ? 2 : 1;
            gameManager.Status = GameStatus.Results;
            gameManager.LoosePlayerIndex = looseCatcher.PlayerIndex;
        }
    }
}