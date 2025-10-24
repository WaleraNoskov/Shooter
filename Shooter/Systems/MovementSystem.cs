using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;

namespace Shooter.Systems;

public class MovementSystem : SystemBase<GameTime>
{
    private readonly QueryDescription _entitiesToMove = new QueryDescription().WithAll<Position, Rigidbody>();
    private readonly Rectangle _viewport;
    
    public MovementSystem(World world) : base(world)
    {
        
    }

    public override void Update(in GameTime time)
    {
        var movement = new Move((float)time.ElapsedGameTime.TotalMilliseconds);
        World.InlineParallelQuery<Move, Position, Rigidbody>(in _entitiesToMove, ref movement);
    }
    
    private readonly struct Move : IForEach<Position, Rigidbody>
    {
        private readonly float _deltaTime;

        public Move(float deltaTime)
        {
            _deltaTime = deltaTime;
        }

        public void Update(ref Position position, ref Rigidbody rigidbody)
        {
            position.Vector += _deltaTime * rigidbody.Velocity;
        }
    }
}