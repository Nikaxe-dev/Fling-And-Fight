using Godot;

namespace FaF.Core;

public abstract partial class GameInstanceAccessor : Node
{
    public GameInstance Game
    {
        get
        {
            Node c = GetOwner();
            while (c != null)
            {
                if (c is GameInstance instance)
                    return instance;
                c = c.GetOwner();
            }
            return null;
        }
    }

    public IServerInstance Server => (IServerInstance)Game;
    public IClientInstance Client => (IClientInstance)Game;
}