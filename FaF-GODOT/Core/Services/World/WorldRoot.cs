using FaF.Core.Visuals;
using Godot;

namespace FaF.Core.Services.World;

public partial class WorldRoot : Node
{
    public StandardCamera3D DebugCamera;

    public override void _Ready()
    {
        base._Ready();

        if (Game.NetworkService.IsServer)
        {
            DebugCamera = new StandardCamera3D()
            {
                Name = "ServerDebugCamera",
                Mode = CameraMode.Free
            };

            AddChild(DebugCamera);
        }
    }
}