using Godot;

namespace FaF.Client;

public interface IClientService
{
    public ClientInstance Client
    {
        get
        {
            if (this is Node node)
                if (node.Owner is ClientInstance client)
                    return client;
            return null;
        }
    }

    public void LoadService();
}