using FaF.Editor.DataModel;
using FaF.Visuals.Camera;
using Godot;
using System;

namespace FaF.Editor;

public partial class EditorViewport : SubViewport
{
    [Export] public PackedScene RepresentationScene;

    public static Node3D WorldRepresentation {private set; get;}

    public static Node3D EnvironmentRepresentation {private set; get;}
    public static WorldEnvironment LightingRepresentation {private set; get;}

    public static MeshInstance3D SunSkyboxDecalRepresentation {private set; get;}
    public static DirectionalLight3D SunRepresentation {private set; get;}

    public static EditorViewport Instance {private set; get;}

    public static FreeCamera FreeCamera;

    public override void _Ready()
    {
        Instance = this;

        WorldRepresentation = RepresentationScene.Instantiate() as Node3D;

        EnvironmentRepresentation = WorldRepresentation.GetNode<Node3D>("Environment");
        LightingRepresentation = WorldRepresentation.GetNode<WorldEnvironment>("Lighting");

        SunRepresentation = LightingRepresentation.GetNode<DirectionalLight3D>("CameraPivot/DirectionalLight");
        SunSkyboxDecalRepresentation = SunRepresentation.GetNode<MeshInstance3D>("Sun");

        AddChild(WorldRepresentation);

        FreeCamera = new();
        AddChild(FreeCamera);
    }
}
