using System.Collections.Generic;
using FaF.Editor.Attributes;

namespace FaF.Editor.DataModel;

#nullable enable

public abstract class Instance
{
    private Instance? _parent;

    [EditorAccess("Parent")]
    public Instance? Parent
    {
        get => _parent;

        set
        {
            Parent?.Children.Remove(this);
            value?.Children.Add(this);

            _parent = value;
        }
    }
    
    private readonly List<Instance> Children = [];

    [EditorAccess("ClassName")]
    public string ClassName {get; private set;}

    [EditorAccess("Name", true)]
    public string Name;

    public Instance()
    {
        ClassName = GetType().Name;
        Name = ClassName;
    }
}