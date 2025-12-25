using Arch.Core;
using Microsoft.Xna.Framework;

namespace Shooter.Systems;

public class PhysicsSystem(World world, nkast.Aether.Physics2D.Dynamics.World physicWorld) : SystemBase<GameTime>(world)
{
    public override void Update(in GameTime gameTime)
    {
        physicWorld.Step(1 / 100f);
    }
}