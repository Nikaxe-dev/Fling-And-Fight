using System;
using System.Collections.Generic;
using FaF.Core.Resources;
using FaF.Core.Registries;
using Godot;

namespace FaF.Core.Services.ProgramServices;

/// <summary>
/// Holds every registry in the current instance of the game. <br/>
/// Anything inheriting from IRegistry is automatically loaded under here. <br/>
/// <br/>
/// To create a new registry, however, use the abstract registry class instead.
/// </summary>
public partial class RegistryHolder : Service, ICoreService, INodeLoader<IRegistry>
{
    private readonly Dictionary<string, IRegistry> Registries = [];

    public override void LoadService()
    {
        base.LoadService();

        ((INodeLoader<IRegistry>)this).LoadDerived();

        foreach (Node node in GetChildren())
            if (node is IRegistry registry)
                Registries[registry.GetType().Name] = registry;
    }

    public T GetRegistry<T>() where T : IRegistry
    {
        if (Registries.TryGetValue(typeof(T).Name, out IRegistry result))
            return (T)result;
        else
            throw new RegistryDoesNotExistException(typeof(T).Name);
    }

    public bool TryGetRegistry<T>(out T registry) where T : IRegistry
    {
        if (Registries.TryGetValue(typeof(T).Name, out IRegistry result))
        {
            registry = (T)result;
            return true;
        } else
        {
            registry = default;
            return false;
        }
    }

    /// <summary>
    /// Checks every registry for one compatible with the type, and if in existance registers the given item. </br>
    /// Outputs whether a registry was found.
    /// </summary>
    public bool AddToRegistry<ContainedType>(Identifier identifier, ContainedType item)
    {
        foreach (Node child in GetChildren())
            if (child is Registry<ContainedType> registry)
            {
                registry.Register(identifier, item);
                return true;
            }
        return false;
    }

    public class RegistryDoesNotExistException(string registryName) : Exception($"Registry {registryName} does not exist.");
}