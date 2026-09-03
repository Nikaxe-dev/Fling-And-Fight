using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FaF.Debug;
using FaF.Editor.Attributes;
using FaF.Game;
using Godot;
using Godot.Collections;

namespace FaF.Editor.DataModel;

#nullable enable

public abstract class Instance
{
    protected static readonly FaFLogger LOGGER = FaFLogger.Get("Editor/DataModel");

    private Instance? _parent;

    // TODO: INSTANCE TYPE NOT SUPPORTED YET
    // [EditorAccess("Parent")]
    public Instance? Parent
    {
        get => _parent;

        set
        {
            Parent?.RemoveChild(this);
            value?.AddChild(this);
        }
    }

    public string FullName
    {
        get
        {
            StringBuilder result = new();
            Instance? c = this;
            while (c != null)
            {
                result.Insert(0, $"{(c.Parent != null ? "." : "")}{c.Name}");
                c = c.Parent;
            }
            return result.ToString();
        }
    }

    public override string ToString()
    {
        return $"{FullName}";
    }

    public event MovedEventHandler? Moved;
    public delegate void MovedEventHandler(Instance? newParent, Instance? oldParent);

    public event RemovedEventHandler? Removed;
    public delegate void RemovedEventHandler();
    
    private readonly List<Instance> _children = [];
    public List<Instance> Children {get => _children;}

    public List<Instance> Descendants {get {
            List<Instance> result = [.. Children];
            foreach (Instance item in result) result.AddRange(item.Descendants);
            return result;
        }
    }

    [EditorAccess("ClassName", true)]
    public string ClassName {get; private set;}

    [EditorAccess("Name"), Save]
    public string Name {get; set;}

    public Instance(bool doAutomaticNodeHandling = true, bool addToSceneTree = true)
    {
        ClassName = GetType().Name;
        Name = ClassName;

        if (doAutomaticNodeHandling) CreateNode(addToSceneTree);
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

        if (child.NodeRepresentation != null) { child.NodeRepresentation.Name = child.Name; NodeRepresentation?.AddChild(child.NodeRepresentation, true); }
            LOGGER.LOG(LogType.DEBUG, $"Instance {child.FullName}.NodeRepresentation (path may be incomplete) ({child.ClassName}) added to or moved around in scenetree.", "NodeHandling");
    }

    public void RemoveChild(Instance child)
    {
        if (child.Parent != this) return;

        Children.Remove(child);
        child._parent = null;

        if (child.NodeRepresentation != null) NodeRepresentation?.RemoveChild(child.NodeRepresentation);
    }

    public T? FindFirstChild<T>(string Name) where T : Instance
    {
        return (T?)Children.Find(c => c.Name == Name);
    }

    public Instance? FindFirstChild(string Name)
    {
        return Children.Find(c => c.Name == Name);
    }

    public void Remove()
    {
        Parent = null;
        NodeRepresentation?.QueueFree();
        Removed?.Invoke();
    }

    /// <summary>
    /// The node that represents the instance. Upon removal of an instance from the datamodel, this is quene freed.
    /// </summary>
    public Node? NodeRepresentation {get; protected set;}

    /// <summary>
    /// Updates non constant properties on the node and its children. Ran right after creating the nodes in CreateNode().
    /// </summary>
    protected virtual void UpdateNode()
    {
        
    }

    protected virtual Node? InternalCreateNode(bool addToTree = true)
    {
        return null;
    }

    /// <summary>
    /// Not all instances create a node.
    /// </summary>
    /// <param name="addToTree">Whether the node is automatically added to the tree.</param>
    /// <returns></returns>
    public Node? CreateNode(bool addToTree = true)
    {
        NodeRepresentation = InternalCreateNode(addToTree);
        LOGGER.LOG(LogType.DEBUG, $"Created node for {FullName} ({ClassName}) (incomplete path, node is not set up)", "NodeHandling");

        if (addToTree && NodeRepresentation != null && Parent != null) NodeRepresentation.GetParent().RemoveChild(NodeRepresentation); Parent?.NodeRepresentation?.AddChild(NodeRepresentation);

        UpdateNode();
        return NodeRepresentation;
    }
}