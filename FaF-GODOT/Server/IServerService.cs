using Godot;

namespace FaF.Server;

public interface IServerService
{
    public ServerInstance Server
    {
        get
        {
            if (this is Node node)
                if (node.Owner is ServerInstance server)
                    return server;
            return null;
        }
    }

    public void LoadService();
}