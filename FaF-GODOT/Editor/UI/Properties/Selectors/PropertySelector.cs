using System;
using System.Reflection;
using FaF.Debug;
using FaF.Editor.Attributes;
using Godot;

# nullable enable

namespace FaF.Editor.UI.Properties.Selectors;

public interface IPropertySelector
{
    object[] ObjectsApplyingTo {get; set;}
    string ObjectPropertyName {get; set;}

    void RefreshVisual();
}

public abstract partial class PropertySelector<T> : HBoxContainer, IPropertySelector
{
    protected static readonly FaFLogger LOGGER = FaFLogger.Get("Editor/UI/Panels/Properties");

    [Export] public required Label KeyLabel;

    public object[] ObjectsApplyingTo {get; set;} = [];
    public string ObjectPropertyName {get; set;} = "unset";

    protected PropertyInfo? propertyInfo;
    protected EditorAccessAttribute? AccessAttribute;

    protected void SetObjectValue(T value)
    {
        foreach (var ObjectApplyingTo in ObjectsApplyingTo)
        {
            if (IsInputValid() && ObjectApplyingTo.GetType().GetProperty(ObjectPropertyName) != null)
            {
                propertyInfo?.SetValue(ObjectApplyingTo, value);
                LOGGER.LOG(LogType.DEBUG, $"Set {ObjectApplyingTo}.{ObjectPropertyName} to {value}", "UserPropertySetting");

                if (ObjectPropertyName == "Name")
                {
                    ExplorerTree.Instance.RefreshTitles();
                }
            }
        }
    }

    protected void OnInputValueChanged()
    {
        if (IsInputValid()) SetObjectValue(GetInputValue());
        else RefreshVisual();
    }

    public override void _Ready()
    {
        // propertyInfo = ObjectApplyingTo?.GetType().GetProperty(ObjectPropertyName);

        foreach (var item in ObjectsApplyingTo)
        {
            propertyInfo = item.GetType().GetProperty(ObjectPropertyName);
            if (propertyInfo != null) break;
        }

        KeyLabel.Text = ObjectPropertyName;
        if (propertyInfo != null) {
            AccessAttribute = (EditorAccessAttribute?)Attribute.GetCustomAttribute(propertyInfo, typeof(EditorAccessAttribute));
            KeyLabel.Text = AccessAttribute?.DisplayName ?? ObjectPropertyName;

            if (AccessAttribute != null && AccessAttribute.ReadOnly) MakeReadonly();
        }

        RefreshVisual();
    }

    public void RefreshVisual()
    {
        if (ObjectsApplyingTo.Length == 1) {
            var value = propertyInfo?.GetValue(ObjectsApplyingTo[0]);
            if (value != null) SetVisualTo((T)value);
        } else SetVisualToDifferent();
    }

    protected abstract T GetInputValue();
    protected abstract void SetVisualTo(T value);
    protected abstract bool IsInputValid();
    protected abstract void MakeReadonly();

    /// <summary>
    /// called when there are multiple objects opened in the properties
    /// </summary>
    protected abstract void SetVisualToDifferent();
}