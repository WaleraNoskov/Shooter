using Arch.Core;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;
using Shooter.Components;
using Shooter.Contracts;
using Shooter.Services;
using World = Arch.Core.World;

namespace Shooter.Systems;

public class SyncSystem(World world, PhysicObjectManager physicObjectManager) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToMove = new QueryDescription().WithAll<ActualMovement>();

    public override void Update(in GameTime gameTime)
    {
        var syncing = new Sync(physicObjectManager);
        World.InlineParallelEntityQuery<Sync, ActualMovement>(in _entitiesToMove, ref syncing);
    }

    private readonly struct Sync(PhysicObjectManager physicObjectManager) : IForEachWithEntity<ActualMovement>
    {
        public void Update(Entity entity, ref ActualMovement actualMovement)
        {
            var entityObjects = physicObjectManager.GetObject(entity);
            var objects = entityObjects?.GetAll<Body>(PhysicObjectTypes.PhysicsBody);
            if (objects is null || objects.Count == 0)
                return;

            var body = objects[0];
            actualMovement.PreviousPosition = actualMovement.Position;
            actualMovement.Position = new Vector2(body.Position.X, body.Position.Y);
            actualMovement.PreviousAngle = actualMovement.Angle;
            actualMovement.Angle = body.Rotation;
        }
    }
}