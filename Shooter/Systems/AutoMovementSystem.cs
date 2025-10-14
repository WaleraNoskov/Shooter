using System.Runtime.CompilerServices;
using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;

namespace Shooter.Systems;

public sealed class MovementSystem : SystemBase<GameTime>
{
    private readonly QueryDescription _entitiesToMove = new QueryDescription().WithAll<Position, Velocity>().WithNone<Input>();
    private readonly Rectangle _viewport;

    public MovementSystem(World world, Rectangle viewport) : base(world)
    {
        _viewport = viewport;
    }

    public override void Update(in GameTime time)
    {
        var movement = new Move((float)time.ElapsedGameTime.TotalMilliseconds);
        World.InlineParallelQuery<Move, Position, Velocity>(in _entitiesToMove, ref movement);
        
        var bouncing = new Bounce(_viewport);
        World.InlineParallelQuery<Bounce, Position, Velocity>(in _entitiesToMove, ref bouncing);
    }

    private readonly struct Move : IForEach<Position, Velocity>
    {
        private readonly float _deltaTime;

        public Move(float deltaTime)
        {
            _deltaTime = deltaTime;
        }

        public void Update(ref Position position, ref Velocity velocity)
        {
            position.Vector += _deltaTime * velocity.Vector;
        }
    }

    private struct Bounce : IForEach<Position, Velocity>
    {
        private Rectangle _viewport;

        public Bounce(Rectangle viewport)
        {
            _viewport = viewport;
        }
        
        public void Update(ref Position position, ref Velocity velocity)
        {
            if(position.Vector.X >= _viewport.X +  _viewport.Width)
                velocity.Vector.X = -velocity.Vector.X;
            if(position.Vector.Y >= _viewport.Y +  _viewport.Height)
                velocity.Vector.Y = -velocity.Vector.Y;
            if(position.Vector.X <= _viewport.X)
                velocity.Vector.X = -velocity.Vector.X;
            if(position.Vector.Y <= _viewport.Y)
                velocity.Vector.Y = -velocity.Vector.Y;
        }
    }
}