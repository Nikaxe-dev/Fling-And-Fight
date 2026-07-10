using System.Linq;
using System.Threading;
using FlingAndFight.Game.Networking;
using Godot;

namespace FlingAndFight.Launcher;

public partial class Initializer : Node
{
    public override void _Ready()
    {
        base._Ready();
        
        // SWITCH to title screen unless --server is enabled, otherwise start a new server with the provided settings (given file OR specified in args OR defaults).
        GD.Print("!!----------------------------------------!!");
        GD.Print("Starting FaF");

        string[] args = OS.GetCmdlineArgs();
        GD.Print("System arguments:");
        GD.Print(args);

        CallDeferred(nameof(QuickPlayArgs), args);
    }

    public void QuickPlayArgs(string[] args)
    {
        if (args.Contains("--server"))
        {
            GD.Print("Starting server");
            
            // TODO: ADD MAP OPTION TO COMMAND LINE SERVER STARTUP (MAKE SURE TO INCLUDE DEFAULTS)

            GetTree().ChangeSceneToFile("res://Content/Maps/Testing_Baseground/Testing_Baseground.tscn");
            NetworkManager.Instance.StartServer(56565);
        } else if (args.Contains("--auto-join"))
        {
            GD.Print("CLIENT: Auto joining server in 2(s).");

            Thread.Sleep(2000);
            GetTree().ChangeSceneToFile("res://Content/Maps/Testing_Baseground/Testing_Baseground.tscn");
            NetworkManager.Instance.StartClient("127.0.0.1", 56565);
        }
    }
}