using System;
using System.Text.Json.Nodes;

namespace FaF.Core.Extensions;

public class JsonKeyNotSetException(Type expectedNativeType, string keyPath) : Exception($"Required {expectedNativeType.Name} value '{keyPath}' is not set.")
{
    public readonly string KeyPath = keyPath;
    public readonly Type ExpectedNativeType = expectedNativeType;
}

public class JsonKeySetToWrongTypeException(Type expectedNativeType, string keyPath) : Exception($"Value '{keyPath}' is the wrong type. The expected type is a {expectedNativeType.Name}.")
{
    public readonly string KeyPath = keyPath;
    public readonly Type ExpectedNativeType = expectedNativeType;
}

public static class JsonNodeExtensions
{
    public static T AsNative<T>(this JsonNode node)
    {
        if (node.AsValue().TryGetValue(out T value))
            return value;
        else
            throw new JsonKeySetToWrongTypeException(typeof(T), node.GetPath());
    }

    public static T GetNodeAsNative<T>(this JsonNode node, string name)
        => node.GetNode<T>(name).AsNative<T>();
    
    public static bool TryGetNodeAsNative<T>(this JsonNode node, string name, out T result)
    {
        result = default;
        node.TryGetNode(name, out JsonNode childNode);

        if (childNode != null)
            result = childNode.AsNative<T>();
        
        return childNode != null;
    }

    public static JsonNode GetNode<NativeTypeContext>(this JsonNode node, string name)
    {
        if (node[name] != null)
            return node[name];
        else
            throw new JsonKeyNotSetException(typeof(NativeTypeContext), name);
    }

    public static bool TryGetNode(this JsonNode node, string name, out JsonNode childNode)
    {
        childNode = null;

        if (node[name] != null)
            childNode = node[name];

        return childNode != null;
    }
}