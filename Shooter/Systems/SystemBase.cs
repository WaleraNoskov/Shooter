using Arch.Core;

namespace Shooter.Systems;

public abstract class SystemBase<T>
{
    protected SystemBase(World world)
    {
        World = world;
    }

    public World World { get; private set; }

    public virtual void Update(in T gameTime)
    {
    }
    
    public virtual void FixedUpdate(in float gameTime)
    {
    }
}