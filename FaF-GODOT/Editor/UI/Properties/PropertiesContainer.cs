using FaF.Debug;
using FaF.Editor.Attributes;
using FaF.Editor.UI;
using FaF.Editor.UI.Properties.Selectors;
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FaF.Editor.UI.Properties;

public partial class PropertiesContainer : VBoxContainer
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Editor/UI/Panels/Properties");

    public override void _Ready()
    {
        Explorer.SelectionChanged += (Selected) => {
            ClearPanel();
            AddItems(Selected);
        };
    }

    private static readonly PackedScene StringSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/StringPropertySelector.tscn");
    private static readonly PackedScene DoubleSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/DoublePropertySelector.tscn");
    private static readonly PackedScene FloatSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/FloatPropertySelector.tscn");
    private static readonly PackedScene IntSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/IntPropertySelector.tscn");
    private static readonly PackedScene ColorSelector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/ColorPropertySelector.tscn");
    private static readonly PackedScene Vector3Selector = ResourceLoader.Load<PackedScene>("Editor/UI/Properties/Selectors/Vector3PropertySelector.tscn");

    private void AddItems(object[] items)
    {
        foreach (object item in items)
        {
            foreach (PropertyInfo property in item.GetType().GetProperties())
            {
                if (Attribute.IsDefined(property, typeof(EditorAccessAttribute)) && GetPropertySelector(property.Name) == null) {
                    if (property.PropertyType == typeof(string))
                    {
                        StringPropertySelector propertySelector = StringSelector.Instantiate<StringPropertySelector>();
                        propertySelector.ObjectsApplyingTo = items;
                        propertySelector.ObjectPropertyName = property.Name;
                        AddChild(propertySelector);
                        PropertySelectors.Add(propertySelector);
                    } else if (property.PropertyType == typeof(double))
                    {
                        DoublePropertySelector propertySelector = DoubleSelector.Instantiate<DoublePropertySelector>();
                        propertySelector.ObjectsApplyingTo = items;
                        propertySelector.ObjectPropertyName = property.Name;
                        AddChild(propertySelector);
                        PropertySelectors.Add(propertySelector);
                    } else if (property.PropertyType == typeof(float))
                    {
                        FloatPropertySelector propertySelector = FloatSelector.Instantiate<FloatPropertySelector>();
                        propertySelector.ObjectsApplyingTo = items;
                        propertySelector.ObjectPropertyName = property.Name;
                        AddChild(propertySelector);
                        PropertySelectors.Add(propertySelector);
                    } else if (property.PropertyType == typeof(int))
                    {
                        IntPropertySelector propertySelector = IntSelector.Instantiate<IntPropertySelector>();
                        propertySelector.ObjectsApplyingTo = items;
                        propertySelector.ObjectPropertyName = property.Name;
                        AddChild(propertySelector);
                        PropertySelectors.Add(propertySelector);
                    } else if (property.PropertyType == typeof(Color))
                    {
                        ColorPropertySelector propertySelector = ColorSelector.Instantiate<ColorPropertySelector>();
                        propertySelector.ObjectsApplyingTo = items;
                        propertySelector.ObjectPropertyName = property.Name;
                        AddChild(propertySelector);
                        PropertySelectors.Add(propertySelector);
                    } else if (property.PropertyType == typeof(Vector3))
                    {
                        Vector3PropertySelector propertySelector = Vector3Selector.Instantiate<Vector3PropertySelector>();
                        propertySelector.ObjectsApplyingTo = items;
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
    }

    private static readonly List<IPropertySelector> PropertySelectors = [];

    public static void RefreshPropertySelectors()
    {
        foreach (var item in PropertySelectors)
        {
            item.RefreshVisual();
        }
    }

    public static void RefreshPropertySelector(string PropertyName) => GetPropertySelector(PropertyName)?.RefreshVisual();
    public static IPropertySelector GetPropertySelector(string PropertyName) => PropertySelectors.Find(i => i.ObjectPropertyName == PropertyName);

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
