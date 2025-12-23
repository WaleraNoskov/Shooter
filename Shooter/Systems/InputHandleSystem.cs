using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;
using Shooter.Contracts;
using Shooter.Services;

namespace Shooter.Systems;

public class InputHandleSystem(World world, InputManager inputManager) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToSetMoving = new QueryDescription().WithAll<UserInput, TargetMovement>();

    public override void Update(in GameTime gameTime)
    {
        var movementSetting = new SetMovement(inputManager);
        World.InlineParallelQuery<SetMovement, UserInput, TargetMovement>(in _entitiesToSetMoving, ref movementSetting);
    }

    private readonly struct SetMovement(InputManager inputManager) : IForEach<UserInput, TargetMovement>
    {
        public void Update(ref UserInput userInput, ref TargetMovement targetMovement)
        {
            var handler = inputManager.Get(targetMovement.Type);
            handler?.SetMovement(ref userInput, ref targetMovement);
            
            targetMovement.NeedToMove = true;
        }
    }
}