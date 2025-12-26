using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shooter.Components;

namespace Shooter.Systems;

public class DrawSystem : SystemBase<GameTime>
{
    private readonly QueryDescription _entitiesToDraw = new QueryDescription()
        .WithAll<ActualMovement, RectangleCollider, Sprite>();

    private readonly SpriteBatch _batch;

    public DrawSystem(World world, SpriteBatch batch) : base(world)
    {
        _batch = batch;
    }

    public override void Update(in GameTime gameTime)
    {
        const int worldToDisplayIndex = 10;
        
        _batch.Begin(samplerState: SamplerState.PointClamp);
        
        var query = World.Query(in _entitiesToDraw);
        foreach (ref var chunk in query)
        {
            chunk.GetSpan<ActualMovement, RectangleCollider, Sprite>(
                out var positions,
                out var rectangleColliders,
                out var sprites);

            foreach (var index in chunk)
            {
                ref var sprite = ref sprites[index];
                ref var collider = ref rectangleColliders[index];
                ref var position = ref positions[index];

                var origin = new Vector2(
                    sprite.Texture.Width / 2f,
                    sprite.Texture.Height / 2f
                );

                var scale = new Vector2(
                    collider.Width * worldToDisplayIndex / sprite.Texture.Width,
                    collider.Height * worldToDisplayIndex / sprite.Texture.Height
                );

                _batch.Draw(
                    sprite.Texture,
                    position.Position * worldToDisplayIndex,
                    null,
                    Color.White,
                    position.Angle,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        _batch.End();
    }
}