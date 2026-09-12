using FaF.Editor.Attributes;
using FaF.Editor.DataModel;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FaF.Editor.UI;

# nullable enable

public partial class NewInstancePanel : PanelContainer
{
    [Export] public required VBoxContainer ButtonsContainer;
    [Export] public required Button CloseButton;
    [Export] public required Label OpenReasonLabel;
    [Export] public required LineEdit SearchInput;

    private static readonly Dictionary<string, Type> InstanceClassMappings =
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(Instance).IsAssignableFrom(t))
            .Where(t => !t.IsAbstract)
            .Where(t => Attribute.IsDefined(t, typeof(CreationAccessAttribute)))
            .ToDictionary(
                t => t.Name,
                t => t
            );

    public static NewInstancePanel? Instance {get; private set;}

    public delegate void UserAcceptedEventHandler(Instance instance);
    public static event UserAcceptedEventHandler? UserAccepted;

    public delegate void UserCanceledEventHandler();
    public static event UserCanceledEventHandler? UserCanceled;

    public void PromptUser(string reason)
    {
        OpenReasonLabel.Text = reason;
        SearchInput.Text = "";
        FilterButtons("");
        SearchInput.GrabFocus();
        Visible = true;
    }

    private void LoadButtons()
    {
        foreach (var (_, value) in InstanceClassMappings)
        {
            Button button = new()
            {
                Text = value.Name,
                Icon = ResourceLoader.Load<Texture2D>($"Editor/DataModelIcons/{value.Name}.png"),
                ExpandIcon = true,
                Name = value.Name,
                Alignment = HorizontalAlignment.Left,
            };

            button.Pressed += () => Accept(value.Name);

            ButtonsContainer.AddChild(button);
        }
    }

    private void FilterButtons(string query)
    {
        foreach (var item in ButtonsContainer.GetChildren())
        {
            if (item is Button button) button.Visible = button.Text.Contains(query, StringComparison.OrdinalIgnoreCase);
        }
    }

    private void Accept(string ClassName)
    {
        Instance? instance = (Instance?)Activator.CreateInstance(InstanceClassMappings[ClassName]);
        if (instance != null) UserAccepted?.Invoke(instance); Visible = false;
    }

    private void Cancel()
    {
        Visible = false;
        UserCanceled?.Invoke();
    }

    public override void _Ready()
    {
        Visible = false;

        Instance = this;

        LoadButtons();
        CloseButton.Pressed += Cancel;

        SearchInput.TextChanged += FilterButtons;
    }

    public override void _Input(InputEvent @event)
    {
        if (Visible && @event.IsActionPressed("ui_accept"))
        {
            foreach (var item in ButtonsContainer.GetChildren())
            {
                if (item is Button button && button.Visible) Accept(button.Text);
            }
            GetViewport().SetInputAsHandled();
        }

        if (Visible && @event.IsActionPressed("ui_cancel"))
        {
            Cancel();
            GetViewport().SetInputAsHandled();
        }
    }
}