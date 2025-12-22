using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shooter.Components;

namespace Shooter.Systems;

public class DrawSystem : SystemBase<GameTime>
{
    private readonly QueryDescription _entitiesToDraw = new QueryDescription().WithAll<ActualMovement, Sprite>();
    private readonly SpriteBatch _batch;

    public DrawSystem(World world, SpriteBatch batch) : base(world)
    {
        _batch = batch;
    }

    public override void Update(in GameTime gameTime)
    {
        _batch.Begin();
        var query = World.Query(in _entitiesToDraw);
        foreach (ref var chunk in query)
        {
            chunk.GetSpan<ActualMovement, Sprite>(out var positions, out var sprites);

            foreach (var index in chunk)
            {
                ref var sprite = ref sprites[index];
                ref var position = ref positions[index];

                var origin = new Vector2(
                    sprite.Texture.Width / 2f,
                    sprite.Texture.Height / 2f
                );

                _batch.Draw(
                    sprite.Texture,
                    position.Position * 10f,
                    null,
                    sprite.Color,
                    position.Angle,
                    origin,
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        _batch.End();
    }
}