namespace FaF.Core.Tree.Game.Implementations;

public partial interface IClientInstance : IGameInstance
{
    public void ConnectTo(string serverIP, int serverPort);
};