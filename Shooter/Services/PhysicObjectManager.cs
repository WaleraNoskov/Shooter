using System.Collections.Generic;
using Arch.Core;
using nkast.Aether.Physics2D.Dynamics;
using Shooter.Contracts;

namespace Shooter.Services;

public class PhysicObjectManager
{
    private readonly Dictionary<Entity, EntityPhysicsObjects> _entities = new();
    private readonly Dictionary<Body, Entity> _bodiesToEntities = new();
    
    public void Add(Entity entity, PhysicObjectTypes type, object obj, string? tag = null)
    {
        if (!_entities.TryGetValue(entity, out var ep))
        {
            ep = new EntityPhysicsObjects();
            _entities[entity] = ep;
        }
        ep.Add(type, obj, tag);

        if (obj is Body body)
        {
            _bodiesToEntities.Add(body, entity);
        }
    }

    public IReadOnlyList<T?> GetAll<T>(Entity entity, PhysicObjectTypes type) where T : class
    {
        return _entities.TryGetValue(entity, out var ep) 
            ? ep.GetAll<T>(type) 
            : [];
    }

    public EntityPhysicsObjects? GetObject(Entity entity) => _entities.GetValueOrDefault(entity);
    
    public Entity? GetEntity(Body body) => _bodiesToEntities.GetValueOrDefault(body);
    
    public T? GetByTag<T>(Entity entity, PhysicObjectTypes type, string tag) where T : class
    {
        if (_entities.TryGetValue(entity, out var ep))
            return ep.GetByTag<T>(type, tag);
        return null;
    }

    public bool Has(Entity entity, PhysicObjectTypes type)
    {
        return _entities.TryGetValue(entity, out var ep) && ep.Has(type);
    }

    public void Remove(Entity entity)
    {
        var epo = _entities.GetValueOrDefault(entity);
        var bodies = epo?.GetAll<Body>(PhysicObjectTypes.PhysicsBody);
        
        if(bodies is not null)
            foreach (var b in bodies)
                _bodiesToEntities.Remove(b);
        
        _entities.Remove(entity);
        
    }
}
