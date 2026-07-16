using FlingAndFight.Game.Networking;
using FlingAndFight.Game.Visuals;
using Godot;

namespace FlingAndFight.Content.Maps.Base;

[GlobalClass]
public partial class MapRoot : Node
{   
    [ExportGroup("Connected Nodes")]
    [Export] public Node3D Scene;

    public FreeCamera DebugCamera;

    public override void _Ready()
    {
        if (Multiplayer.IsServer())
        {
            DebugCamera = new FreeCamera
            {
                Name = "ServerDebugCamera"
            };
            
            Scene.AddChild(DebugCamera);
        }
    }
}