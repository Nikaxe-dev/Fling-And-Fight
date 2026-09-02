using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FaF.Editor.Attributes;
using FaF.Game;
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

    public List<Instance> Descendants {get {
            List<Instance> result = [.. Children];
            foreach (Instance item in result) result.AddRange(item.Descendants);
            return result;
        }
    }

    [EditorAccess("ClassName")]
    public string ClassName {get; private set;}

    [EditorAccess("Name", true), Save]
    public string Name {get; set;}

    public Instance()
    {
        ClassName = GetType().Name;
        Name = ClassName;

        if (GameManager.IS_IN_EDITOR) CreateNode();
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

        if (child.Parent?.ClassName != "Environment") child.NodeRepresentation?.GetParentOrNull<Node>()?.RemoveChild(child.NodeRepresentation);
        if (child.NodeRepresentation != null) NodeRepresentation?.AddChild(child.NodeRepresentation);
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

        if (addToTree && NodeRepresentation != null && Parent != null) NodeRepresentation.GetParent().RemoveChild(NodeRepresentation); Parent?.NodeRepresentation?.AddChild(NodeRepresentation);

        UpdateNode();
        return NodeRepresentation;
    }
}