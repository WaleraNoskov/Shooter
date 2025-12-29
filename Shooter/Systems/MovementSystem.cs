using System;
using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;
using Shooter.Services;

namespace Shooter.Systems;

public class MovementSystem(World world, MovementManager movementManager, PhysicObjectManager physicObjectManager) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToMove = new QueryDescription().WithAll<TargetMovement>();

    public override void FixedUpdate(in float gameTime)
    {
        var moving = new Move(gameTime, movementManager, physicObjectManager);
        World.InlineParallelEntityQuery<Move, TargetMovement>(in _entitiesToMove, ref moving);
    }

    private readonly struct Move(
        float elapsedTime,
        MovementManager movementManager,
        PhysicObjectManager physicObjectManager)
        : IForEachWithEntity<TargetMovement>
    {
        public void Update(Entity entity, ref TargetMovement targetMovement)
        {
            if (!targetMovement.NeedToMove)
                return;
            
            var handler = movementManager.Get(targetMovement.Type);
            if (handler is null)
                return;

            var objects = physicObjectManager.GetObject(entity);
            if (objects is null)
                return;
            
            handler.Move(elapsedTime, ref targetMovement, objects);
            
            targetMovement.NeedToMove = false;
        }
    }
}