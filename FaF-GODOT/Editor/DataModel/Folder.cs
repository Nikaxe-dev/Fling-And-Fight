using FaF.Editor.Attributes;
using FaF.Editor.DataModel.Physical;
using Godot;

namespace FaF.Editor.DataModel;

[CreationAccess]
public class Folder : Instance
{
    protected override Node InternalCreateNode(bool addToTree = true)
    {
        return new();
    }
}