using FaF.Editor.DataModel.Physical;
using Godot;

namespace FaF.Editor.DataModel;

# nullable enable

public class World : Instance
{
    protected override Node? InternalCreateNode(bool addToTree = true)
    {
        return EditorRoot.WORLD_REPRESENTATION;
    }
}