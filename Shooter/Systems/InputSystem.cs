using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Shooter.Components;

namespace Shooter.Systems;

public class InputSystem : SystemBase<GameTime>
{
    private readonly QueryDescription _entitiesToSetInput = new QueryDescription().WithAll<Input>();

    public InputSystem(World world) : base(world)
    {
    }

    public override void Update(in GameTime time)
    {
        var settingInput = new SetInput((float)time.ElapsedGameTime.TotalMilliseconds);
        World.InlineParallelQuery<SetInput, Input>(in _entitiesToSetInput, ref settingInput);
    }

    private readonly struct SetInput : IForEach<Input>
    {
        private readonly float _deltaTime;

        public SetInput(float deltaTime)
        {
            _deltaTime = deltaTime;
        }

        public void Update(ref Input input)
        {
            if (input.PlayerIndex == 1)
            {
                int x = 0, y = 0;

                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    y = 1;
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    y = -1;
                else
                    y = 0;

                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    x = 1;
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    x = -1;
                else
                    x = 0;

                input.X = x;
                input.Y = y;
            }
            else if (input.PlayerIndex == 2)
            {
                int x = 0, y = 0;

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    y = 1;
                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                    y = -1;
                else
                    y = 0;

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    x = 1;
                else if (Keyboard.GetState().IsKeyDown(Keys.A))
                    x = -1;
                else
                    x = 0;

                input.X = x;
                input.Y = y;
            }
        }
    }
}