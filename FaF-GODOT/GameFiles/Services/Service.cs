using System.Collections.Generic;
using System.Collections.ObjectModel;
using FaF.Debug;
using Godot;

namespace FaF.Services;

public partial class Service(string[] dependencies) : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Services");

    public readonly ReadOnlyCollection<string> Dependencies = dependencies.AsReadOnly();
    public virtual void LoadService() => LOGGER.LOG(Enums.LogType.INFO, $"Loading {Name}", "ServiceLoading", true);
}