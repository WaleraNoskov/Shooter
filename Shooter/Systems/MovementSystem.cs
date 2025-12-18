using System;
using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;
using Shooter.Contracts;
using Shooter.Services;

namespace Shooter.Systems;

public class MovementSystem(World world, MovementManager movementManager, PhysicObjectManager physicObjectManager) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToMove = new QueryDescription().WithAll<Input, Movement>();

    public override void Update(in GameTime gameTime)
    {
        var moving = new Move(gameTime.ElapsedGameTime, movementManager, physicObjectManager);
        World.InlineParallelEntityQuery<Move, Input, Movement>(in _entitiesToMove, ref moving);
    }

    private readonly struct Move(
        TimeSpan elapsedTime,
        MovementManager movementManager,
        PhysicObjectManager physicObjectManager)
        : IForEachWithEntity<Input, Movement>
    {
        public void Update(Entity entity, ref Input input, ref Movement movement)
        {
            var handler = movementManager.Get(movement.Type);
            if (handler is null)
                return;

            var objects = physicObjectManager.GetObject(entity);
            if (objects is null)
                return;
            
            handler.Move(elapsedTime, ref input, ref movement, objects);
        }
    }
}