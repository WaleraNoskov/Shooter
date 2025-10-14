using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shooter.Components;

namespace Shooter.Systems;

public class DrawSystem : SystemBase<GameTime>
{
    private readonly QueryDescription _entitiesToDraw = new QueryDescription().WithAll<Position, Sprite>();
    private readonly SpriteBatch _batch;

    public DrawSystem(World world, SpriteBatch batch) : base(world)
    {
        _batch = batch;
    }

    public override void Update(in GameTime state)
    {
        _batch.Begin();
        var query = World.Query(in _entitiesToDraw);
        foreach (ref var chunk in query)
        {
            chunk.GetSpan<Position, Sprite>(out var positions, out var sprites);

            foreach (var index in chunk)
            {
                ref var position = ref positions[index];
                ref var sprite = ref sprites[index];
                _batch.Draw(sprite.Texture, position.Vector, sprite.Color);
            }
        }
        
        _batch.End();
    }
}