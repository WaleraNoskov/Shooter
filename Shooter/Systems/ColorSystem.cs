using System;
using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;

namespace Shooter.Systems;

public class ColorSystem : SystemBase<GameTime>
{
    private readonly QueryDescription _entitiesToChangeColor = new QueryDescription().WithAll<Sprite>();
    private static GameTime? _gameTime;
    private static Random _random;
    
    public ColorSystem(World world)
        : base(world)
    {
        _random = new Random();
    }
    
    public override void Update(in GameTime time)
    {
        _gameTime = time;

        // Modifies the color of all entities fitting the entitiesToChangeColor query.
        World.Query(in _entitiesToChangeColor, (ref Sprite sprite) =>
        {
            sprite.Color.R += (byte)(_gameTime.ElapsedGameTime.TotalMilliseconds * 0.08);
            sprite.Color.G += (byte)(_gameTime.ElapsedGameTime.TotalMilliseconds * 0.08);
            sprite.Color.B += (byte)(_gameTime.ElapsedGameTime.TotalMilliseconds * 0.08);
        });

        // A demonstration of bulk adding and removing components.
        World.Add(in _entitiesToChangeColor, _random.Next());
        World.Remove<int>(in _entitiesToChangeColor);
    }
}