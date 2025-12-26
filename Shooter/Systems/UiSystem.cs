using Arch.Core;
using Microsoft.Xna.Framework;
using Shooter.Components;

namespace Shooter.Systems;

public class UiSystem(World world) : SystemBase<GameTime>(world)
{
    private readonly QueryDescription _entitiesToDisplay = new QueryDescription().WithAll<Player>();
    
    public override void Update(in GameTime gameTime)
    {
        var query = World.Query(in _entitiesToDisplay);
        foreach (ref var chunk in query)
        {
            chunk.GetSpan<Player>();

            foreach (var index in chunk)
            {
                var player = chunk.Get<Player>(index);
                
            }
        }
    }
}