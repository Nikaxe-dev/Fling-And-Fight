using System.Collections.Generic;

namespace FaF.Editor.DataModel;

#nullable enable

public abstract class Instance
{
    private Instance? _parent;

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

    public string ClassName {get; private set;}
    public string Name;

    public Instance()
    {
        ClassName = GetType().Name;
        Name = ClassName;
    }
}