using Arch.Core;
using nkast.Aether.Physics2D.Dynamics.Contacts;

namespace Shooter.EventComponents;

public struct CollisionEvent
{
    public Entity EntityA  { get; set; }
    public Entity EntityB { get; set; }
    public Contact Contact { get; set; }
}