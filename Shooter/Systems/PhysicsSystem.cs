using System;
using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace Shooter.Systems;

public class PhysicsSystem(World world, nkast.Aether.Physics2D.Dynamics.World physicWorld) : SystemBase<GameTime>(world)
{
    public override void Update(in GameTime gameTime)
    {
        physicWorld.Step(1 / 100f);
    }
}