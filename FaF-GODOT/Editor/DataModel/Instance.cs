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
            Parent?._children.Remove(this);
            value?._children.Add(this);

            _parent = value;
        }
    }
    
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

    public string ToJson() => JsonSerializer.Serialize(this, SerializerOptions);

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public void SetProperty(string PropertyName, object Value)
    {
        Type type = GetType();
        PropertyInfo? propertyInfo = type.GetProperty(PropertyName);
        propertyInfo?.SetValue(this, Value);
    }

    public void AddChild(Instance child)
    {
        child.Parent = this;
    }
}