using FaF.Core;
using Godot;

namespace FaF.Saves;

/// <summary>
/// Manages the saving and loading for a game. <br/>
/// Has methods for saving or loading player or world data. <br/>
/// 
/// Saving player data is done on the clients whilst world data is done on the server (most of the time the same computer as the player who is hosting). <br/>
/// 
/// <br/>
/// 
/// <b>Never</b> save important progression or permission data on the player save. This should either be handled by third party services (such as steam) or through saving it on the world save.
/// </summary>
/// <seealso cref="DataSaver"/>
public partial class SaveManager : Manager<SaveManager>
{
    
}