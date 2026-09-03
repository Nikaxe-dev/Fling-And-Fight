using FaF.Debug;
using FaF.Editor.Attributes;
using FaF.Editor.UI;
using FaF.Editor.UI.Properties.Selectors;
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

// TODO: ADD PROPER SUPPORT FOR MULTI SELECTING

public partial class Properties : VBoxContainer
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Editor/UI/Panels/Properties");

    public override void _Ready()
    {
        Explorer.SelectionChanged += (Selected) => {
            ClearPanel();
            foreach (var item in Selected)
            {
                AddObject(item);
            }
        };
    }

    private static readonly PackedScene StringSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/StringPropertySelector.tscn");
    private static readonly PackedScene DoubleSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/DoublePropertySelector.tscn");
    private static readonly PackedScene FloatSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/FloatPropertySelector.tscn");
    private static readonly PackedScene IntSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/IntPropertySelector.tscn");
    private static readonly PackedScene ColorSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/ColorPropertySelector.tscn");
    private static readonly PackedScene Vector3Selector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/Vector3PropertySelector.tscn");

    private void AddObject(object item)
    {
        foreach (PropertyInfo property in item.GetType().GetProperties())
        {
            if (Attribute.IsDefined(property, typeof(EditorAccessAttribute))) {
                if (property.PropertyType == typeof(string))
                {
                    StringPropertySelector propertySelector = StringSelector.Instantiate<StringPropertySelector>();
                    propertySelector.ObjectApplyingTo = item;
                    propertySelector.ObjectPropertyName = property.Name;
                    AddChild(propertySelector);
                    PropertySelectors.Add(propertySelector);
                } else if (property.PropertyType == typeof(double))
                {
                    DoublePropertySelector propertySelector = DoubleSelector.Instantiate<DoublePropertySelector>();
                    propertySelector.ObjectApplyingTo = item;
                    propertySelector.ObjectPropertyName = property.Name;
                    AddChild(propertySelector);
                    PropertySelectors.Add(propertySelector);
                } else if (property.PropertyType == typeof(float))
                {
                    FloatPropertySelector propertySelector = FloatSelector.Instantiate<FloatPropertySelector>();
                    propertySelector.ObjectApplyingTo = item;
                    propertySelector.ObjectPropertyName = property.Name;
                    AddChild(propertySelector);
                    PropertySelectors.Add(propertySelector);
                } else if (property.PropertyType == typeof(int))
                {
                    IntPropertySelector propertySelector = IntSelector.Instantiate<IntPropertySelector>();
                    propertySelector.ObjectApplyingTo = item;
                    propertySelector.ObjectPropertyName = property.Name;
                    AddChild(propertySelector);
                    PropertySelectors.Add(propertySelector);
                } else if (property.PropertyType == typeof(Color))
                {
                    ColorPropertySelector propertySelector = ColorSelector.Instantiate<ColorPropertySelector>();
                    propertySelector.ObjectApplyingTo = item;
                    propertySelector.ObjectPropertyName = property.Name;
                    AddChild(propertySelector);
                    PropertySelectors.Add(propertySelector);
                } else if (property.PropertyType == typeof(Vector3))
                {
                    Vector3PropertySelector propertySelector = Vector3Selector.Instantiate<Vector3PropertySelector>();
                    propertySelector.ObjectApplyingTo = item;
                    propertySelector.ObjectPropertyName = property.Name;
                    AddChild(propertySelector);
                    PropertySelectors.Add(propertySelector);
                } else
                {
                    LOGGER.LOG(LogType.WARNING, $"Property of type {property.PropertyType} and name {property.Name} in {item.GetType()} is not supported by the properties panel. Please report this.", "PropertyUILoading", true);
                }
            }
        }
    }

    private static readonly List<IPropertySelector> PropertySelectors = [];

    public static void RefreshPropertySelectors()
    {
        foreach (var item in PropertySelectors)
        {
            item.RefreshVisual();
        }
    }

    public static void RefreshPropertySelector(string PropertyName)
    {
        PropertySelectors.Find(i => i.ObjectPropertyName == PropertyName)?.RefreshVisual();
    }

    public static void RefreshTransformationPropertySelectors()
    {
        RefreshPropertySelector("Position");
        RefreshPropertySelector("Rotation");
        RefreshPropertySelector("GlobalPosition");
        RefreshPropertySelector("GlobalRotation");
    }

    private void ClearPanel()
    {
        foreach (var item in GetChildren())
        {
            PropertySelectors.Remove(item as IPropertySelector);
            item.QueueFree();
        }
    }
}
