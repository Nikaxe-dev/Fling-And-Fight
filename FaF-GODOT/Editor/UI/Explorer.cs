using FaF.Debug;
using FaF.Editor;
using FaF.Editor.DataModel;
using FaF.Editor.UI.Properties;
using FaF.UserInput;
using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaF.Editor.UI;

public partial class Explorer : Tree
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Editor/UI/Panels/Explorer");

    public static Explorer Instance {get; private set;}

    public static readonly List<Instance> SelectedInstances = [];

    public delegate void SelectionChangedEventHandler(Instance[] Selected);
    public static event SelectionChangedEventHandler SelectionChanged;

    private readonly Dictionary<Instance, TreeItem> TreeMappings = [];
    private readonly Dictionary<TreeItem, Instance> InstanceMappings = [];

    private Instance GetTreeItemInstance(TreeItem item)
    {
        if (item == null) return null;
        InstanceMappings.TryGetValue(item, out Instance res);
        return res;
    }

    private TreeItem GetInstanceTreeItem(Instance instance)
    {
        if (instance == null) return null;
        TreeMappings.TryGetValue(instance, out TreeItem res);
        return res;
    }

    private void AddInstance(Instance instance, int Position = -1)
    {
        TreeItem item = CreateItem(GetInstanceTreeItem(instance.Parent), Position);
        item.SetText(0, instance.Name);

        Texture2D Icon = ResourceLoader.Load<Texture2D>("res://Editor/DataModelIcons".PathJoin($"{instance.ClassName}.png"));
        item.SetIcon(0, Icon);

        item.SetEditable(0, true);

        TreeMappings[instance] = item;
        InstanceMappings[item] = instance;

        instance.Moved += (newParent, oldParent) =>
        {
            item.GetParent()?.RemoveChild(item);
            GetInstanceTreeItem(newParent)?.AddChild(item);
        };

        instance.Removed += () =>
        {
            item.Free();
        };

        foreach (Instance child in instance.Children)
        {
            AddInstance(child);
        }

        item.SetCollapsedRecursive(true);
    }

    private void LoadExplorer()
    {
        LOGGER.LOG(LogType.INFO, "Loading Explorer");

        DeselectAll();
        Clear();

        AddInstance(EditorRoot.Instance.MapRoot);
        GetInstanceTreeItem(EditorRoot.Instance.MapRoot).Collapsed = false;
    }

    public override void _Ready()
    {
        Instance = this;

        EditorRoot.Instance.WorldLoaded += (WorldID) => {
            LoadExplorer();
        };

        // renaming
        ItemEdited += () =>
        {
            GetTreeItemInstance(GetEdited()).Name = GetEdited().GetText(0);
        };

        MultiSelected += (item, column, selected) =>
        {
            Instance instance = GetTreeItemInstance(item);
            if (instance == null) return;

            if (selected && !SelectedInstances.Contains(instance)) SelectedInstances.Add(instance);
            else if (!selected) SelectedInstances.Remove(instance);
            
            SelectionChanged?.Invoke([.. SelectedInstances]);
        };
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // TODO: Add confirmation popup to deletion
        if (@event.IsActionPressed("editor_remove"))
        {
            TreeItem next = GetNextSelected(null);
            while (next != null)
            {
                var current = next;
                next = GetNextSelected(next);
                GetTreeItemInstance(current).Remove();
            }
        }
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        Godot.Collections.Array items = [];
        TreeItem next = GetNextSelected(null);
        
        VBoxContainer v = new();
        v.Theme = Theme;

        while (next != null)
        {
            items.Add(next);

            HBoxContainer h = new();
            
            TextureRect icon = new();
            icon.Texture = next.GetIcon(0);
            icon.CustomMaximumSize = new Vector2(25,25);

            Label title = new();
            title.Text = next.GetText(0);

            h.AddChild(icon);
            h.AddChild(title);

            v.AddChild(h);
            
            next = GetNextSelected(next);
        }

        SetDragPreview(v);
        return items;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        DropModeFlags = 1;

        int DropSection = GetDropSectionAtPosition(atPosition);
        if (DropSection == -100) return false;

        TreeItem item = GetItemAtPosition(atPosition);
        if (data.AsGodotArray().Contains(item)) return false;

        return true;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        int DropSection = GetDropSectionAtPosition(atPosition);
        TreeItem OtherItem = GetItemAtPosition(atPosition);
        Instance OtherInstance = GetTreeItemInstance(OtherItem);

        TreeItem previousItem = null;
        foreach (TreeItem item in data.AsGodotArray().Select(v => (TreeItem)(GodotObject)v))
        {
            Instance instance = GetTreeItemInstance(item);

            instance.Parent = OtherInstance;

            OtherItem.Collapsed = false;

            previousItem = item;
        }
    }
}
