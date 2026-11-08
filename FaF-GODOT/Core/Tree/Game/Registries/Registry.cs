using System;
using System.Linq;
using FaF.Core.Resources;
using FaF.Core.Tree.Game.Nodes;

namespace FaF.Core.Tree.Game.Registries;

public class RegistryItemNotFoundException(IRegistry registry, Identifier identifier)
    : Exception($"{identifier} was not found in registry {registry.GetType().Name}")
{
    public readonly IRegistry Registry = registry;
    public readonly Identifier Identifier = identifier;
}

/// <summary>
/// Only used for the RegistryHolder to provide a reference to the registry class that isnt generic. <br/>
/// Does not contain any members so use the Registry class for everything else.
/// </summary>
public interface IRegistry
{
    
}

public abstract partial class Registry<T> : GameInstanceAccessor, IRegistry
{
    public abstract bool Exists(Identifier identifier);
    public abstract T GetItem(Identifier identifier);

    public abstract Identifier[] GetIdentifiers();
    public abstract T[] GetItems();

    public virtual bool TryGetItem(Identifier identifier, out T item)
    {
        bool itemExists = Exists(identifier);
        item = itemExists ? GetItem(identifier) : default;
        return itemExists;
    }

    public virtual bool Has(Identifier identifier)
        => GetIdentifiers().Contains(identifier);

    public virtual void Register(Identifier identifier, T item) {}
}