using Godot;

namespace FaF.Core;

public interface IGameService
{
    public GameInstance Game
    {
        get
        {
            if (this is Node node)
                if (node.Owner is GameInstance game)
                    return game;
            return null;
        }
    }

    public void LoadService();
}