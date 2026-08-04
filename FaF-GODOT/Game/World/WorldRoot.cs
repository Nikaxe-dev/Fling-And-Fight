using FaF.Visuals.Camera;
using Godot;

namespace FaF.Game.World;

/// <summary>
/// The root of the loaded world. Controls the worlds global state.
/// </summary>
[GlobalClass]
public partial class WorldRoot : Node
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