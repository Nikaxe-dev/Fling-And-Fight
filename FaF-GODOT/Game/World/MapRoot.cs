using FaF.Visuals.Camera;
using Godot;

namespace FaF.Game.World;

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