using System.Collections.Generic;
using FaF.Core.Data.ContentResources;

namespace FaF.Core.Data.Registries;

public abstract partial class NativeRegistry<T> : Registry<T>
{
    private readonly Dictionary<Identifier, T> Items = [];

    public override void Register(Identifier identifier, T item)
    {
        if (item is ContentResource resource)
            resource.Identifier = identifier;
        Items[identifier] = item;
    }

    public override T GetItem(Identifier identifier)
    {
        if (Exists(identifier))
            return Items[identifier];
        else
            throw new RegistryItemNotFoundException(this, identifier);
    }

    public override bool Exists(Identifier identifier)
        => Items.ContainsKey(identifier);
}