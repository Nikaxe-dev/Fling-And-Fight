using System.Linq;
using System.Threading;
using FaF.Networking;
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
			
			// TODO: Add map option to command line server startup (MAKE SURE TO INCLUDE DEFAULTS)

			GetTree().ChangeSceneToFile("res://Data/Worlds/FaF_World/Data/Maps/New_Sedes/New_Sedes_Scene.tscn");
			NetworkManager.Instance.StartServer(NetworkManager.DEFAULT_PORT);
		} else if (args.Contains("--auto-join"))
		{
			GD.Print("CLIENT: Auto joining server in 2(s).");

			Thread.Sleep(2000);
			GetTree().ChangeSceneToFile("res://Data/Worlds/FaF_World/Data/Maps/New_Sedes/New_Sedes_Scene.tscn");
			NetworkManager.Instance.JoinServer(NetworkManager.DEFAULT_IP, NetworkManager.DEFAULT_PORT);
		}
	}
}
