using Godot;

namespace FlingAndFight.Game.Networking;

public partial class NetworkManager : Node
{
    public static NetworkManager Instance {get; private set;}

    public ENetMultiplayerPeer Peer;

    public override void _Ready()
    {
        Instance = this;
    }

    public void StartServer(int PORT, int MAX_CHANNELS = 0)
    {
        Peer = new ENetMultiplayerPeer();
        Peer.CreateServer(PORT, 30, MAX_CHANNELS);
        Multiplayer.MultiplayerPeer = Peer;

        GD.Print("Started Server");
    }

    public void StartClient(string IP_ADDRESS, int PORT)
    {
        Peer = new ENetMultiplayerPeer();
        Peer.CreateClient(IP_ADDRESS, PORT);
        Multiplayer.MultiplayerPeer = Peer;

        GD.Print("Started Client");
    }
}