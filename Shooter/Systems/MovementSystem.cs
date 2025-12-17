using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;

namespace Shooter.Systems;

public class MovementSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToMove = new QueryDescription().WithAll<Input>();

    public override void Update(in GameTime gameTime)
    {
        var moving = new Move((float)gameTime.ElapsedGameTime.TotalMilliseconds);
        World.InlineParallelQuery<Move, Input>(in _entitiesToMove, ref moving);
    }
    
    private readonly struct Move(float deltaTime) : IForEach<Input>
    {
        public void Update(ref Input input)
        {
            
        }
    }
}