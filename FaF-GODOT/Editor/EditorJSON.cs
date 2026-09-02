using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using FaF.Debug;
using FaF.Editor.Attributes;
using FaF.Editor.DataModel;
using FaF.Editor.DataModel.Physical.World;
using Godot;

namespace FaF.Editor;

public static class EditorJSON
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Editor/JSON");

    public static JsonNode ToJson(object Object)
    {
        if (Object is Instance instance) return JsonFromInstance(instance);
        else if (Object is Vector2 vector2) return new JsonObject {["ClassName"] = "Vector2", ["X"] = vector2.X, ["Y"] = vector2.Y};
        else if (Object is Vector3 vector3) return new JsonObject {["ClassName"] = "Vector3", ["X"] = vector3.X, ["Y"] = vector3.Y, ["Z"] = vector3.Z};
        else if (Object is Vector4 vector4) return new JsonObject {["ClassName"] = "Vector4", ["X"] = vector4.X, ["Y"] = vector4.Y, ["Z"] = vector4.Z, ["W"] = vector4.W};
        else if (Object is Basis basis) return new JsonObject {["ClassName"] = "Basis", ["X"] = ToJson(basis.X), ["Y"] = ToJson(basis.Y), ["Z"] = ToJson(basis.Z)};
        else if (Object is Transform3D transform) return new JsonObject {["ClassName"] = "Transform3D", ["Basis"] = ToJson(transform.Basis), ["Origin"] = ToJson(transform.Origin)};
        else if (Object is Color color) return new JsonObject {["ClassName"] = "Color", ["Hex"] = color.ToHtml()};
        else return JsonFromBasicType(Object);
    }

    private static JsonValue JsonFromBasicType(object Object)
    {
        return JsonValue.Create(Object);
    }

    private static JsonObject JsonFromInstance(Instance instance)
    {
        JsonObject ROOT = new()
        {
            ["ClassName"] = "Instance",
            ["InstanceClass"] = instance.ClassName,
        };

        JsonObject Properties = [];
        
        foreach (PropertyInfo property in instance.GetType().GetProperties())
        {
            if (Attribute.IsDefined(property, typeof(SaveAttribute)) && property?.GetValue(instance) != property?.GetValue((Instance)Activator.CreateInstance(InstanceClassMappings[instance.ClassName])))
            {
                SaveAttribute saveAttribute = (SaveAttribute)Attribute.GetCustomAttribute(property, typeof(SaveAttribute));
                Properties.Add(saveAttribute?.KeyName ?? property.Name, ToJson(property.GetValue(instance)));
            }
        }

        ROOT.Add("Properties", Properties);

        if (instance.Children.Count > 0)
        {
            JsonArray Children = [];
            foreach (Instance child in instance.Children) Children.Add(JsonFromInstance(child));
            ROOT.Add("Children", Children);
        }

        return ROOT;
    }
    
    public static T FromJson<T>(JsonElement Root)
    {
        string ClassName = Root.GetProperty("ClassName").GetString();
        if (ClassName == null) {LOGGER.LOG(LogType.ERROR, $"Expected String in 'ClassName' of data. Full JSON: ${Root}", "ConversionFrom"); return default;}

        object result = ClassName switch
        {
            "Vector2" => new Vector2(
                (float)Root.GetProperty("X").GetDouble(),
                (float)Root.GetProperty("Y").GetDouble()
            ),

            "Vector3" => new Vector3(
                (float)Root.GetProperty("X").GetDouble(),
                (float)Root.GetProperty("Y").GetDouble(),
                (float)Root.GetProperty("Z").GetDouble()
            ),

            "Vector4" => new Vector4(
                (float)Root.GetProperty("X").GetDouble(),
                (float)Root.GetProperty("Y").GetDouble(),
                (float)Root.GetProperty("Z").GetDouble(),
                (float)Root.GetProperty("W").GetDouble()
            ),

            "Basis" => new Basis(
                FromJson<Vector3>(Root.GetProperty("X")),
                FromJson<Vector3>(Root.GetProperty("Y")),
                FromJson<Vector3>(Root.GetProperty("Z"))
            ),

            "Transform3D" => new Transform3D(
                FromJson<Basis>(Root.GetProperty("Basis")),
                FromJson<Vector3>(Root.GetProperty("Origin"))
            ),

            "Color" => new Color(Root.GetProperty("Hex").GetString()),

            "ResourceReference" => ResourceLoader.Load(Root.GetProperty("FilePath").GetString()),

            "Instance" => InstanceFromJson(Root),

            _ => throw new JsonException($"Unknown ClassName '{ClassName}'.")
        };

        return (T) result;
    }

    private static readonly Dictionary<string, Type> InstanceClassMappings =
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(Instance).IsAssignableFrom(t))
            .Where(t => !t.IsAbstract)
            .ToDictionary(
                t => t.Name,
                t => t
            );
    
    private static object JsonElementToObject(JsonElement Root) => Root.ValueKind switch
    {
        JsonValueKind.Undefined => null,
        JsonValueKind.String => Root.GetString(),
        JsonValueKind.Number => (float)Root.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        _ => throw new JsonException($"JsonElementToObject(): {Root.ValueKind} cannot be converted into a C# type. Full JSON: {Root}")
    };

    private static Instance InstanceFromJson(JsonElement Root)
    {
        string InstanceClass = Root.GetProperty("InstanceClass").GetString();
        if (InstanceClass == null) {LOGGER.LOG(LogType.ERROR, $"Expected String in 'InstanceClass' of data. Full JSON: ${Root}", "ConversionFrom"); return default;}

        Instance instance = (Instance)Activator.CreateInstance(InstanceClassMappings[InstanceClass]);

        foreach (var property in instance.GetType().GetProperties())
        {
            SaveAttribute saveAttribute = (SaveAttribute)Attribute.GetCustomAttribute(property, typeof(SaveAttribute));
            string KeyName = saveAttribute?.KeyName ?? property.Name;

            if (saveAttribute != null) {
                if (Root.GetProperty("Properties").TryGetProperty(KeyName, out JsonElement element)) {
                    if (element.ValueKind == JsonValueKind.Object)
                    {
                        instance.SetProperty(property.Name, FromJson<object>(element));
                    } else
                    {
                        instance.SetProperty(property.Name, JsonElementToObject(element));
                    }
                }
            }
        }

        if (Root.TryGetProperty("Children", out JsonElement ChildrenElement)) {
            JsonElement.ArrayEnumerator ChildrenEnumerator = ChildrenElement.EnumerateArray();

            foreach (JsonElement childJSON in ChildrenEnumerator)
            {
                InstanceFromJson(childJSON).Parent = instance;
            }
        }

        return instance;
    }
}