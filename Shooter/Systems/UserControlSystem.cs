using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;

namespace Shooter.Systems;

public class UserControlSystem : SystemBase<GameTime>
{
    private readonly QueryDescription _entitiesToSetInput = new QueryDescription().WithAll<Input, Rigidbody>();
    
    public UserControlSystem(World world) : base(world)
    {
    }

    public override void Update(in GameTime time)
    {
        var settingInput = new SetInput((float)time.ElapsedGameTime.TotalMilliseconds);
        World.InlineParallelQuery<SetInput, Input, Rigidbody>(in _entitiesToSetInput, ref settingInput);
    }
    
    private readonly struct SetInput : IForEach<Input, Rigidbody>
    {
        private readonly float _deltaTime;

        public SetInput(float deltaTime)
        {
            _deltaTime = deltaTime;
        }

        public void Update(ref Input input, ref Rigidbody rigidbody)
        {
            rigidbody.Velocity = new Vector2(input.X, input.Y) * rigidbody.MaxVelocity;
        }
    }
}