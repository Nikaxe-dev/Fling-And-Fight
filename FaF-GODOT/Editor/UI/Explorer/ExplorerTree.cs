using FaF.Debug;
using FaF.Editor.DataModel;
using FaF.Editor.UI.Properties;
using Godot;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace FaF.Editor.UI;

public partial class ExplorerTree : Tree
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Editor/UI/Panels/Explorer");

    public static ExplorerTree Instance {get; private set;}

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
            SelectedInstances.Remove(instance);
            SelectionChanged.Invoke([.. SelectedInstances]);
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

    private void PromptAddChild(Instance instance)
    {
        void onAccept(Instance child)
        {
            child.Parent = instance;
            AddInstance(child);
            SelectInstance(child);

            NewInstancePanel.UserAccepted -= onAccept;
            NewInstancePanel.UserCanceled -= onCancel;
        }

        void onCancel() {
            NewInstancePanel.UserCanceled -= onCancel;
            NewInstancePanel.UserAccepted -= onAccept;
        }

        NewInstancePanel.Instance.PromptUser($"Add child to {instance.FullName}");

        NewInstancePanel.UserAccepted += onAccept;
        NewInstancePanel.UserCanceled += onCancel;
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
            PropertiesContainer.RefreshPropertySelector("Name");
        };

        ButtonClicked += (item, column, id, mouse_button_index) =>
        {
            if (id == 0) PromptAddChild(GetTreeItemInstance(item));
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

    public void SelectInstance(Instance instance)
    {
        if ((SelectedInstances.Count == 1 || Input.IsActionPressed("editor_select_multiple")) && SelectedInstances.Contains(instance))
        {
            GetInstanceTreeItem(instance).Deselect(0);
            SelectedInstances.Remove(instance);
            SelectionChanged.Invoke([.. SelectedInstances]);
            return;
        }
        
        if (!Input.IsActionPressed("editor_select_multiple")) DeselectAll();

        if (!SelectedInstances.Contains(instance)) {
            GetInstanceTreeItem(instance)?.Select(0, true);
            GetInstanceTreeItem(instance)?.UncollapseTree();
            SelectedInstances.Add(instance);
            SelectionChanged.Invoke([.. SelectedInstances]);
        }
    }

    public static TreeItem MouseHoveredItem {get; private set;}

    private static readonly Texture2D AddChildButtonIcon = ResourceLoader.Load<Texture2D>("Editor/UI/Explorer/AddChild.png");

    public override void _Process(double delta)
    {
        TreeItem CurrentItem = GetItemAtPosition(GetLocalMousePosition());

        if (CurrentItem != MouseHoveredItem && IsInstanceValid(MouseHoveredItem))
        {
            MouseHoveredItem?.EraseButton(0,0);
            MouseHoveredItem = CurrentItem;
            MouseHoveredItem?.AddButton(0, AddChildButtonIcon, 0, tooltipText: "Add child");
        }
    }

    public void RefreshTitles()
    {
        foreach (var (key, value) in TreeMappings)
        {
            value.SetText(0, key.Name);
        }
    }

    public new void DeselectAll()
    {
        base.DeselectAll();
        SelectedInstances.RemoveAll(i => true);
        SelectionChanged.Invoke([]);
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
            GetViewport().SetInputAsHandled();
        }

        if (@event.IsActionPressed("editor_add_child"))
        {
            TreeItem next = GetNextSelected(null);
            if (next != null) PromptAddChild(GetTreeItemInstance(next));
            else PromptAddChild(EditorRoot.Instance.MapRoot.FindFirstChild<DataModel.Environment>("Environment"));
            GetViewport().SetInputAsHandled();
        }

        if (@event.IsActionPressed("ui_cancel"))
        {
            DeselectAll();
            GetViewport().SetInputAsHandled();
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
