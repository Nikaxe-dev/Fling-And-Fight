namespace FaF.Core;

public partial interface IClientInstance : IGameInstance
{
    public void ConnectTo(string serverIP, int serverPort);
};