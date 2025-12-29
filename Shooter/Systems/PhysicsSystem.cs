using Arch.Core;
using Microsoft.Xna.Framework;

namespace Shooter.Systems;

public class PhysicsSystem(World world, nkast.Aether.Physics2D.Dynamics.World physicWorld) : SystemBase<GameTime>(world)
{

    public override void FixedUpdate(in float gameTime)
    {
        physicWorld.Step(gameTime);
    }
}