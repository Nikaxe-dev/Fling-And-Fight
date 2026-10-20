using System;
using FaF.Core.Resources;
using Godot;

namespace FaF.Core.Data.Registries;

public class RegistryItemNotFoundException(IRegistry registry, Identifier identifier)
    : Exception($"{identifier} was not found in registry {registry.GetType().Name}");

/// <summary>
/// Only used for the RegistryHolder to provide a reference to the registry class that isnt generic. <br/>
/// Does not contain any members so use the Registry class for everything else.
/// </summary>
public interface IRegistry
{
    
}

public abstract partial class Registry<T> : Node, IRegistry
{
    public abstract bool Exists(Identifier identifier);
    public abstract T GetItem(Identifier identifier);

    public virtual bool TryGetItem(Identifier identifier, out T item)
    {
        bool itemExists = Exists(identifier);
        item = itemExists ? GetItem(identifier) : default;
        return itemExists;
    }

    public virtual void Register(Identifier identifier, T item) {}
}