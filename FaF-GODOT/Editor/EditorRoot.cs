using System.Text.Json;
using FaF.Debug;
using FaF.Editor.DataModel;
using FaF.Editor.DataModel.Physical;
using FaF.Editor.DataModel.Physical.World;
using FaF.Game;
using Godot;

namespace FaF.Editor;

public partial class EditorRoot : Node
{
    private readonly FaFLogger LOGGER = FaFLogger.Get("Editor/Root");

    public EditorRoot Instance;

    public override void _Ready()
    {
        Instance = this;

        GameManager.Instance.EmitSignal(GameManager.SignalName.FaFEditorLoaded);

        LOGGER.LOG(LogType.INFO, "TEST: Converting from JSON", "Tests", true);
        string LongTestJSON = "{\"ClassName\":\"Instance\",\"InstanceClass\":\"World\",\"Children\":[{\"ClassName\":\"Instance\",\"InstanceClass\":\"Environment\",\"Children\":[{\"ClassName\":\"Instance\",\"InstanceClass\":\"Part\",\"Properties\":{\"Name\":\"Baseplate\",\"Shape\":\"Block\",\"Material\":\"SmoothPlastic\",\"Transform\":{\"ClassName\":\"Transform3D\",\"Origin\":{\"ClassName\":\"Vector3\",\"X\":0,\"Y\":0,\"Z\":0},\"Basis\":{\"ClassName\":\"Basis\",\"X\":{\"ClassName\":\"Vector3\",\"X\":1,\"Y\":0,\"Z\":0},\"Y\":{\"ClassName\":\"Vector3\",\"X\":0,\"Y\":1,\"Z\":0},\"Z\":{\"ClassName\":\"Vector3\",\"X\":0,\"Y\":0,\"Z\":1}}},\"Size\":{\"ClassName\":\"Vector3\",\"X\":75,\"Y\":2,\"Z\":75}}},{\"ClassName\":\"Instance\",\"InstanceClass\":\"SpawnLocation\",\"Properties\":{\"Name\":\"WorldSpawn\",\"Transform\":{\"ClassName\":\"Transform3D\",\"Origin\":{\"ClassName\":\"Vector3\",\"X\":0,\"Y\":0,\"Z\":0},\"Basis\":{\"ClassName\":\"Basis\",\"X\":{\"ClassName\":\"Vector3\",\"X\":1,\"Y\":0,\"Z\":0},\"Y\":{\"ClassName\":\"Vector3\",\"X\":0,\"Y\":1,\"Z\":0},\"Z\":{\"ClassName\":\"Vector3\",\"X\":0,\"Y\":0,\"Z\":1}}}}},{\"ClassName\":\"Instance\",\"InstanceClass\":\"Part\",\"Properties\":{\"Name\":\"WorldSpawnVisual\",\"Transform\":{\"ClassName\":\"Transform3D\",\"Origin\":{\"ClassName\":\"Vector3\",\"X\":0,\"Y\":0,\"Z\":0},\"Basis\":{\"ClassName\":\"Basis\",\"X\":{\"ClassName\":\"Vector3\",\"X\":1,\"Y\":0,\"Z\":0},\"Y\":{\"ClassName\":\"Vector3\",\"X\":0,\"Y\":1,\"Z\":0},\"Z\":{\"ClassName\":\"Vector3\",\"X\":0,\"Y\":0,\"Z\":1}}},\"Size\":{\"ClassName\":\"Vector3\",\"X\":1,\"Y\":0.25,\"Z\":1}}}],\"Properties\":{\"Name\":\"Environment\"}},{\"ClassName\":\"Instance\",\"InstanceClass\":\"Lighting\",\"Properties\":{\"Name\":\"Lighting\",\"Skybox\":{\"ClassName\":\"ResourceReference\",\"FilePath\":\"res://Editor/Samples/BasicEnvironment.tres\"}}}],\"Properties\":{\"Name\":\"World\"}}";
        GD.Print(LongTestJSON);
        World world = EditorJSON.FromJson<World>(JsonDocument.Parse(LongTestJSON).RootElement);
        LOGGER.LOG(LogType.INFO, "TEST: Converting back to JSON", "Tests", true);
        GD.Print(EditorJSON.ToJson(world));
    }
}
