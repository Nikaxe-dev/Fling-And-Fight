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

            button.Pressed += () =>
            {
                Instance? instance = (Instance?)Activator.CreateInstance(InstanceClassMappings[value.Name]);
                if (instance != null) UserAccepted?.Invoke(instance); Visible = false;
            };

            ButtonsContainer.AddChild(button);
        }
    }


    public override void _Ready()
    {
        Visible = false;

        Instance = this;

        LoadButtons();
        CloseButton.Pressed += () =>
        {
            Visible = false;
            UserCanceled?.Invoke();
        };
    }
}