using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FaF.Editor.Attributes;
using Godot;
using Godot.Collections;

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
            Parent?.RemoveChild(this);
            value?.AddChild(this);
        }
    }

    public event MovedEventHandler? Moved;
    public delegate void MovedEventHandler(Instance? newParent, Instance? oldParent);

    public event RemovedEventHandler? Removed;
    public delegate void RemovedEventHandler();
    
    private readonly List<Instance> _children = [];
    public List<Instance> Children {get => _children;}

    [EditorAccess("ClassName")]
    public string ClassName {get; private set;}

    [EditorAccess("Name", true), Save]
    public string Name {get; set;}

    public Instance()
    {
        ClassName = GetType().Name;
        Name = ClassName;
    }

    public void SetProperty(string PropertyName, object Value)
    {
        Type type = GetType();
        PropertyInfo? propertyInfo = type.GetProperty(PropertyName);
        propertyInfo?.SetValue(this, Value);
    }

    public void AddChild(Instance child)
    {
        if (child.Parent != null) return;
        if (child.Parent == this) return;

        child.Moved?.Invoke(this, child.Parent);

        Children.Add(child);
        child._parent = this;
    }

    public void RemoveChild(Instance child)
    {
        if (child.Parent != this) return;

        Children.Remove(child);
        child._parent = null;
    }

    public void Remove()
    {
        Parent = null;
        Removed?.Invoke();
    }
}