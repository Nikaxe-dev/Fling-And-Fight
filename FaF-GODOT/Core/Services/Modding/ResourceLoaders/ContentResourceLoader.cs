using System;
using System.Text.Json;
using FaF.Core.Debug;
using FaF.Core.FileSystem;
using FaF.Core.Services.Content.Resources;

namespace FaF.Core.Services.Modding.ResourceLoaders;

public interface IContentResourceLoader<ResourceType>
{
    public static abstract ResourceType Load(JsonElement element);
    public static abstract ResourceType Load(FileInstance file, string contentNamespace = null);
}

public class ContentResourceLoader<ResourceType, ThisType> : IContentResourceLoader<ResourceType>
    where ThisType : ContentResourceLoader<ResourceType, ThisType>, new()
    where ResourceType : ContentResource, new()
{
    public static readonly ThisType Loader = new();

    public static ResourceType Load(JsonElement element)
        => Loader._Load(new(), element);
    
    public static ResourceType Load(FileInstance file, string contentNamespace = null)
    {
        try {
            var resource = Loader._Load(new(), file.ReadJson().RootElement);
            resource.ID = file.Name;
            resource.FILE_PATH = file.Path;

            if (contentNamespace != null)
                resource.NAMESPACE = contentNamespace;

            return resource;
        } catch (Exception exception)
        {
            throw new FileFailedToLoadException(file, exception);
        }
    }

    protected virtual ResourceType _Load(ResourceType resource, JsonElement element) => resource;

    protected object LoadMetaProperty(JsonElement element, string keyName, JsonValueKind expectedKind)
    {
        if (TryLoadMetaProperty(out object loaded, element, keyName, expectedKind))
            return loaded;
        else
            throw new JsonRequiredKeyNotFilledException(typeof(ResourceType).Name, keyName);
    }

    protected bool TryLoadMetaProperty(out object result, JsonElement element, string keyName, JsonValueKind expectedKind)
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
            }
            else
                throw new JsonRequiredKeyWrongTypeException(expectedKind, property.ValueKind, typeof(ResourceType).Name, keyName);
        result = null;
        return false;
    }

    public class FileFailedToLoadException(FileInstance file, Exception exception) : Exception($"Failed to load {typeof(ResourceType).Name} file '{file}':\n\t{exception.Message}");

    public class JsonRequiredKeyNotFilledException(string resourceType, string keyName) : Exception($"Required property {resourceType}.{keyName} is not filled.");
    public class JsonRequiredKeyWrongTypeException(JsonValueKind expectedType, JsonValueKind givenType, string resourceType, string keyName) : Exception($"Property {resourceType}.{keyName} was set to value of kind {givenType} instead of the expected {expectedType}.");
}