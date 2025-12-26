using System;
using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shooter.Components;

namespace Shooter.Systems;

public class DrawSystem(World world, SpriteBatch batch) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToDraw = new QueryDescription()
        .WithAll<ActualMovement, RectangleCollider, Sprite>();

    public float Alpha { get; set; }

    public override void Update(in GameTime gameTime)
    {
        const int worldToDisplayIndex = 10;

        batch.Begin(samplerState: SamplerState.PointWrap);

        var drawing = new Draw(batch, Alpha);
        World.InlineParallelQuery<Draw, ActualMovement, RectangleCollider, Sprite>(_entitiesToDraw, ref drawing);

        batch.End();
    }

    private readonly struct Draw(SpriteBatch batch, float alpha) : IForEach<ActualMovement, RectangleCollider, Sprite>
    {
        public void Update(ref ActualMovement actualMovement, ref RectangleCollider collider, ref Sprite sprite)
        {
            const int worldToDisplayIndex = 10;

            var interpolatedPosition = Vector2.Lerp(
                actualMovement.PreviousPosition * worldToDisplayIndex,
                actualMovement.Position * worldToDisplayIndex,
                alpha
            );
            var pixelSnappedPosition = new Vector2(
                MathF.Round(interpolatedPosition.X),
                MathF.Round(interpolatedPosition.Y)
            );

            var interpolatedAngle = MathHelper.Lerp(
                actualMovement.PreviousAngle * worldToDisplayIndex,
                actualMovement.Angle * worldToDisplayIndex,
                alpha
            );

            var origin = new Vector2(
                sprite.Texture.Width / 2f,
                sprite.Texture.Height / 2f
            );

            var scale = new Vector2(
                collider.Width * worldToDisplayIndex / sprite.Texture.Width,
                collider.Height * worldToDisplayIndex / sprite.Texture.Height
            );

            batch.Draw(
                sprite.Texture,
                interpolatedPosition,
                null,
                Color.White,
                interpolatedAngle,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}