using System;
using System.Text.Json;
using FaF.Core.Lib.FileSystem;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public partial class ContentResourceLoaderExtension<ResourceType, ThisType>
    where ResourceType : ContentResource, new()
    where ThisType : ContentResourceLoaderExtension<ResourceType, ThisType>, new()
{
    public static readonly ThisType Loader = new();

    protected virtual ResourceType LoadThis(ResourceType resource, JsonElement element, string filePath = null) => resource;

    public static ResourceType Load(JsonElement element, string filePath = null)
        => Loader.LoadThis(new(), element);
    
    public static ResourceType LoadFile(FileInstance file)
        => Load(file.ReadJson().RootElement, file.Path);
    
    protected object LoadMetaProperty(JsonElement element, string keyName, JsonValueKind expectedKind, string filePath = null)
    {
        if (TryLoadMetaProperty(out object loaded, element, keyName, expectedKind, filePath))
            return loaded;
        else
            throw new JsonKeyNotFilledException(typeof(ResourceType).Name, keyName);
    }

    protected bool TryLoadMetaProperty(out object result, JsonElement element, string keyName, JsonValueKind expectedKind, string filePath = null)
    {
        if (element.TryGetProperty(keyName, out JsonElement property))
            if (property.ValueKind == expectedKind)
            {
                result = property.ValueKind switch
                {
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Number => property.GetDouble(),
                    JsonValueKind.String => property.GetString(),
                    JsonValueKind.Array => property,
                    JsonValueKind.Object => property,
                    _ => null
                };

                return true;
            } else
                throw new JsonKeyWrongTypeException(expectedKind, property.ValueKind,  typeof(ResourceType).Name, keyName, filePath);
        
        result = null;
        return false;
    }

    public class JsonKeyNotFilledException(string resourceType, string keyName, string filePath = null) : Exception($"Required property {resourceType}.{keyName}{(filePath == null ? "" : $" in {filePath}")} is not filled.");
    public class JsonKeyWrongTypeException(JsonValueKind expectedKind, JsonValueKind givenKind, string resourceType, string keyName, string filePath = null) : Exception($"Property {resourceType}.{keyName}{(filePath == null ? "" : $" in {filePath}")} was set to value of kind {givenKind} instead of the expected {expectedKind}.");
}