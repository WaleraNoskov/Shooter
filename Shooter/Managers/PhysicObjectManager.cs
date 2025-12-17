using System;
using System.Collections.Generic;
using Arch.Core;
using Shooter.Contracts;

namespace Shooter.Managers;

public class PhysicObjectManager
{
    private readonly Dictionary<Entity, EntityPhysicsObjects> _entities = new();

    public void Add(Entity entity, PhysicObjectTypes type, object obj, string tag = null)
    {
        if (!_entities.TryGetValue(entity, out var ep))
        {
            ep = new EntityPhysicsObjects();
            _entities[entity] = ep;
        }
        ep.Add(type, obj, tag);
    }

    public IReadOnlyList<T> GetAll<T>(Entity entity, PhysicObjectTypes type) where T : class
    {
        return _entities.TryGetValue(entity, out var ep) 
            ? ep.GetAll<T>(type) 
            : [];
    }

    public T GetByTag<T>(Entity entity, PhysicObjectTypes type, string tag) where T : class
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
        _entities.Remove(entity);
    }
}
