using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Shooter.Components;

namespace Shooter.Systems;

public class UserInputSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToSetInput = new QueryDescription().WithAll<UserInput>();

    public override void FixedUpdate(in float gameTime)
    {
        var settingInput = new SetInput();
        World.InlineParallelQuery<SetInput, UserInput>(in _entitiesToSetInput, ref settingInput);
    }

    private readonly struct SetInput : IForEach<UserInput>
    {

        public void Update(ref UserInput userInput)
        {
            if (userInput.PlayerIndex == 1)
            {
                int x, y;

                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    y = -1;
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    y = 1;
                else
                    y = 0;

                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    x = 1;
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    x = -1;
                else
                    x = 0;

                userInput.X = x;
                userInput.Y = y;
            }
            else if (userInput.PlayerIndex == 2)
            {
                int x = 0, y = 0;

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    y = -1;
                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                    y = 1;
                else
                    y = 0;

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    x = 1;
                else if (Keyboard.GetState().IsKeyDown(Keys.A))
                    x = -1;
                else
                    x = 0;

                userInput.X = x;
                userInput.Y = y;
            }
        }
    }
}