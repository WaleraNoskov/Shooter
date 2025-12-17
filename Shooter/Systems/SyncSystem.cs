using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;

namespace Shooter.Systems;

public class SyncSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToMove = new QueryDescription().WithAll<Position>();

    public override void Update(in GameTime gameTime)
    {
        var syncing = new Sync((float)gameTime.ElapsedGameTime.TotalMilliseconds);
        World.InlineParallelQuery<Sync, Position>(in _entitiesToMove, ref syncing);
    }

    private readonly struct Sync(float deltaTime) : IForEach<Position>
    {
        private readonly float _deltaTime = deltaTime;

        public void Update(ref Position position)
        {
            // position.Vector = new Vector2(physicBody.Body.Position.X, physicBody.Body.Position.Y);
            // position.Angle = physicBody.Body.Rotation;
        }
    }
}