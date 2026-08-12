using FaF.Debug;
using FaF.Game;
using Godot;
using System;

namespace FaF.Editor;

public partial class EditorRoot : Node
{
    private readonly FaFLogger LOGGER = FaFLogger.Get("Editor/Manager");

    public EditorRoot Instance;

    public override void _Ready()
    {
        Instance = this;

        GameManager.Instance.EmitSignal(GameManager.SignalName.FaFEditorLoaded);
    }
}
