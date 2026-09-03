using System;
using System.Reflection;
using FaF.Debug;
using FaF.Editor.Attributes;
using Godot;

# nullable enable

namespace FaF.Editor.UI.Properties.Selectors;

public interface IPropertySelector
{
    object? ObjectApplyingTo {get; set;}
    string ObjectPropertyName {get; set;}

    void RefreshVisual();
}

public abstract partial class PropertySelector<T> : HBoxContainer, IPropertySelector
{
    protected static readonly FaFLogger LOGGER = FaFLogger.Get("Editor/UI/Panels/Properties");

    [Export] public required Label KeyLabel;

    public object? ObjectApplyingTo {get; set;}
    public string ObjectPropertyName {get; set;} = "unset";

    protected PropertyInfo? propertyInfo;

    protected void SetObjectValue(T value)
    {
        if (IsInputValid()) propertyInfo?.SetValue(ObjectApplyingTo, value); LOGGER.LOG(LogType.DEBUG, $"Set {ObjectApplyingTo}.{ObjectPropertyName} to {value}", "UserPropertySetting");
    }

    protected void OnInputValueChanged()
    {
        if (IsInputValid()) SetObjectValue(GetInputValue());
        else
        {
            var value = propertyInfo?.GetValue(ObjectApplyingTo);
            if (value != null) SetVisualTo((T)value);
        }
    }

    public override void _Ready()
    {
        propertyInfo = ObjectApplyingTo?.GetType().GetProperty(ObjectPropertyName);

        KeyLabel.Text = ObjectPropertyName;
        if (propertyInfo != null) {
            EditorAccessAttribute? accessAttribute = (EditorAccessAttribute?)Attribute.GetCustomAttribute(propertyInfo, typeof(EditorAccessAttribute));
            KeyLabel.Text = accessAttribute?.DisplayName ?? ObjectPropertyName;
        }

        RefreshVisual();
    }

    public void RefreshVisual()
    {
        var value = propertyInfo?.GetValue(ObjectApplyingTo);
        if (value != null) SetVisualTo((T)value);
    }

    protected abstract T GetInputValue();
    protected abstract void SetVisualTo(T value);
    protected abstract bool IsInputValid();
}