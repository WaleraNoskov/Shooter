using Shooter.Components;

namespace Shooter.Contracts;

public interface IInputHandler
{
    void SetMovement(ref UserInput input, ref TargetMovement targetMovement);
}