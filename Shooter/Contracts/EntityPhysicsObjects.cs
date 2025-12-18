using System;
using System.Collections.Generic;
using System.Linq;

namespace Shooter.Contracts;

public class EntityPhysicsObjects
{
    private readonly Dictionary<PhysicObjectTypes, List<PhysicsObjectEntry>> _objects = new();

    // Добавить объект с опциональным тегом
    public void Add(PhysicObjectTypes type, object obj, string? tag = null)
    {
        if (!_objects.TryGetValue(type, out var list))
        {
            list = new List<PhysicsObjectEntry>();
            _objects[type] = list;
        }

        list.Add(new PhysicsObjectEntry(obj, tag));
    }

    // Получить все объекты типа
    public IReadOnlyList<T> GetAll<T>(PhysicObjectTypes type) where T : class
    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return _objects.TryGetValue(type, out var list)
            ? list.Select(x => x.Object as T).Where(x => x != null).ToList()
            : [];
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    // Получить объект по тегу
    public T? GetByTag<T>(PhysicObjectTypes type, string tag) where T : class
    {
        if (!_objects.TryGetValue(type, out var list)) 
            return null;

        if (list.All(x => x.Tag != tag))
            return null;
        
        var entry = list.FirstOrDefault(x => x.Tag == tag);
        return entry.Object as T;
    }

    public bool Has(PhysicObjectTypes type) => _objects.ContainsKey(type);

    public void Remove(PhysicObjectTypes type, string? tag = null)
    {
        if (!_objects.TryGetValue(type, out var list)) return;

        if (tag == null)
        {
            _objects.Remove(type);
        }
        else
        {
            list.RemoveAll(x => x.Tag == tag);
            if (list.Count == 0)
                _objects.Remove(type);
        }
    }
}