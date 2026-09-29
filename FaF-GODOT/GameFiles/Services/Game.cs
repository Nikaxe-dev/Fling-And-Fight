using System;
using System.Collections.Generic;
using FaF.Services.Lifecycle;
using FaF.Services.UserInput;
using FaF.Services.World;
using Godot;

namespace FaF.Services;

public partial class Game : Node
{
    public static Game Instance {get; private set;}

    #region State

    private static readonly List<string> LoadedServices = [];

    #endregion

    #region Service Definitions

    public static readonly NetworkService NetworkService = new() {Name = "NetworkService"};
    public static readonly InputService InputService = new() {Name = "InputService"};
    public static readonly MouseInputService MouseInputService = new() {Name = "MouseInputService"};
    public static readonly RunService RunService = new() {Name = "RunService"};
    public static readonly WorldService WorldService = new() {Name = "WorldService"};
    public static readonly PlayerService PlayerService = new() {Name = "PlayerService"};

    private void AddServices()
    {
        AddChild(NetworkService);
        AddChild(RunService);

        AddChild(WorldService);
        AddChild(PlayerService);

        AddChild(InputService);
        AddChild(MouseInputService);
    }

    private void LoadService(Service service)
    {
        if (LoadedServices.Contains(service.Name))
            return;
        
        foreach (string serviceName in service.Dependencies)
        {
            if (GetNode(serviceName) is Service dependency)
                LoadService(dependency);
        }
        
        LoadedServices.Add(service.Name);
        service.LoadService();
    }

    private void LoadServices()
    {
        foreach (var child in GetChildren())
        {
            if (child is Service service)
                LoadService(service);
        }
    }

    #endregion

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        
        AddServices();
        LoadServices();
    }
}