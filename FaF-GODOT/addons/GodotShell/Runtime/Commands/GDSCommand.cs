namespace GodotShell.Runtime.Commands;

public class GDSCommand(string name, string description) : GDSNode
{
    public string Name = name;
    public string Description = description;
}