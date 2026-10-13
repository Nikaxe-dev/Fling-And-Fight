namespace FaF.Core;

public interface IClientInstance
{
    public void ConnectTo(string serverIP, int serverPort);
};