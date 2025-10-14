using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;

namespace Shooter.Systems;

public class ManualMovementSystem : SystemBase<GameTime>
{
    private readonly QueryDescription _entitiesToMove = new QueryDescription().WithAll<Input, Position, Velocity>();
    private readonly Rectangle _viewport;
    
    public ManualMovementSystem(World world) : base(world)
    {
    }
    
    public override void Update(in GameTime time)
    {
        var movement = new Move((float)time.ElapsedGameTime.TotalMilliseconds);
        World.InlineParallelQuery<Move, Input, Position, Velocity>(in _entitiesToMove, ref movement);
    }
    
    private readonly struct Move : IForEach<Input, Position, Velocity>
    {
        private readonly float _deltaTime;

        public Move(float deltaTime)
        {
            _deltaTime = deltaTime;
        }

        public void Update(ref Input input, ref Position position, ref Velocity velocity)
        {
            position.Vector += new Vector2(input.X, -input.Y) * _deltaTime * velocity.Vector;
        }
    }
}