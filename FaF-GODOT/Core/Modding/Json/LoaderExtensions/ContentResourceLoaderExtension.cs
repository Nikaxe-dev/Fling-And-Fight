using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using FaF.Core.Extensions;
using FaF.Core.Lib.FileSystem;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public partial class ContentResourceLoaderExtension<ResourceType, ThisType>
    where ResourceType : ContentResource, new()
    where ThisType : ContentResourceLoaderExtension<ResourceType, ThisType>, new()
{
    public static readonly ThisType Loader = new();

    protected virtual ResourceType LoadThis(ResourceType resource, JsonNode node) => resource;

    public static ResourceType Load(JsonNode node, string filePath = null)
    {
        try {
            return Loader.LoadThis(new(), node);
        } catch (Exception exception)
        {
            if (exception is JsonKeyNotSetException keyNotSetException)
                throw new ContentResourceJsonKeyNotSetException(keyNotSetException.ExpectedNativeType, filePath, keyNotSetException.KeyPath);
            
            if (exception is JsonKeySetToWrongTypeException keySetToWrongTypeException)
                throw new ContentResourceJsonKeySetToWrongTypeException(keySetToWrongTypeException.ExpectedNativeType, filePath, keySetToWrongTypeException.KeyPath);

            throw exception;
        }
    }
    
    public static ResourceType LoadFile(FileInstance file)
        => Load(file.AsJsonNode(), file.Path);
    
    public class ContentResourceJsonKeyNotSetException(Type expectedNativeType, string filePath, string keyPath) : Exception($"{filePath} : {typeof(ResourceType).Name} - required {expectedNativeType.Name} value '{keyPath}' is not set.");
    public class ContentResourceJsonKeySetToWrongTypeException(Type expectedNativeType, string filePath, string keyPath) : Exception($"{filePath} : {typeof(ResourceType).Name} - value '{keyPath}' is has the wrong type. The expected type is a {expectedNativeType.Name}.");
}