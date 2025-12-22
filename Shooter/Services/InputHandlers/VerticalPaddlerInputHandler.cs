using System;
using System.Numerics;
using Shooter.Components;
using Shooter.Contracts;

namespace Shooter.Services.InputHandlers;

public class VerticalPaddlerInputHandler : IInputHandler
{
    public void SetMovement(ref UserInput input, ref TargetMovement targetMovement)
    {
        if (input.Y == 0)
        {
            targetMovement.Velocity = 0;
            targetMovement.Direction = Vector2.Zero;
        }
        else
        {
            targetMovement.Velocity = Math.Abs(targetMovement.MaxVelocity * input.Y);
            targetMovement.Direction = new Vector2(0, input.Y > 0 ? 1 : -1);
        }
        
        targetMovement.NeedToMove = true;
    }
}