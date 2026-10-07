using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FaF.Debug;
using Godot;

namespace FaF.Services;

public abstract partial class Service(string[] dependencies) : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Services");

    public readonly ReadOnlyCollection<string> Dependencies = dependencies.AsReadOnly();

    public string GetFullServiceName()
    {
        StringBuilder result = new();
        Node c = this;
        while (c != null)
        {
            if (c is Service)
                result.Insert(0, $"{((c.GetParent() != null && c.GetParent() is Service) ? "." : "")}{c.Name}");
            c = c.GetParent();
        }
        return result.ToString();
    }

    protected virtual void _LoadService() {}
    public virtual void LoadService()
    {
        var startTime = Time.GetTicksMsec();
        _LoadService();
        IsLoaded = true;

        LOGGER.LOG(Enums.LogType.INFO, $"Loaded service {GetFullServiceName()} in {Time.GetTicksMsec() - startTime}ms", "ServiceLoading", true);
    }

    public bool IsLoaded {get; private set;} = false;
}