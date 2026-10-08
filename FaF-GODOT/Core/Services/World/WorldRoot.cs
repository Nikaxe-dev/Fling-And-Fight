using FaF.Visuals.Camera;
using Godot;

namespace FaF.Services.World;

public partial class WorldRoot : Node
{
    public FreeCamera DebugCamera;

    public override void _Ready()
    {
        base._Ready();

        if (Game.NetworkService.IsServer)
        {
            DebugCamera = new FreeCamera()
            {
                Name = "ServerDebugCamera"
            };

            AddChild(DebugCamera);
        }
    }
}