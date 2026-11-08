using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using FaF.Core.Lib.FileSystem;
using FaF.Core.Tree.Game.Modding.JsonExtensions;
using FaF.Core.Resources;
using FaF.Core.Resources.Content;
using Godot;
using FaF.Core.Tree.Game.Modding.Implementations;

namespace FaF.Core.Tree.Game.Modding.Extensions;

/// <summary>
/// Extend this to populate a registry with the specified resource type from json data files. <br/>
/// Requires a ContentResourceJsonExtension that accepts the given resource type to compile the files into the proper resource type. <br/>
/// <br/>
/// Supports merging / overwriting files in namespaces through adding a file extension. <br/>
/// <br/>
/// .merge - Merges the data. (best for slight edits and compatibility) <br/>
/// .overwrite - Overwrites the data. (should only be used when absolutely needed, this will break compatibility with other mods changing this data) <br/>
/// unspecified - Taken as the first declaration of the data. There may only be one of these. <br/>
/// </summary>
public abstract partial class RegistryPopulatorModExtension<ResourceType, JsonExtension>(string fileExtension, string registryFolderJoinedPath = "data", bool includeNamespace = true) : ModExtension(), IModNamespaceExtension, IModLoaderEndExtension
    where ResourceType : ContentResource, new()
    where JsonExtension : ContentResourceJsonExtension<ResourceType, JsonExtension>, new()
{
    public WorldResource LoadingWorld { get; set; }

    private Dictionary<string, (string declarationFile, List<string> mergeFiles, List<string> overwriteFiles)> definitionsData = [];
    private List<(Identifier identifier, JsonObject compiledJson, string declarationFilePath)> compiledJsonData = [];

    private void DiscoverFilesRecursive(FolderInstance registryFolder, string namespaceID)
    {
        foreach (FSInstance item in registryFolder)
        {
            if (item is FileInstance file)
            {
                if (file.IsJsonExtension() && file.HasExtension(fileExtension))
                {
                    Identifier identifier = new(namespaceID, file.Name);

                    if (!definitionsData.TryGetValue(identifier.ToString(), out (string declarationFile, List<string> mergeFiles, List<string> overwriteFiles) data))
                        data = (null, [], []);

                    if (file.HasExtension("merge"))
                        data.mergeFiles.Add(file.Path);
                    else if (file.HasExtension("overwrite"))
                        data.overwriteFiles.Add(file.Path);
                    else
                        data.declarationFile = file.Path;

                    definitionsData[identifier.ToString()] = data;
                }
            } else if (item is FolderInstance folder)
                DiscoverFilesRecursive(folder, namespaceID);
        }
    }

    private JsonObject MergeJson(JsonObject a, JsonObject b)
    {
        JsonObject result = (JsonObject)a.DeepClone();

        foreach (var (key, bNode) in b)
        {
            if (result[key] is JsonObject aObject && bNode is JsonObject bObject)
                result[key] = MergeJson(aObject, bObject);

            result[key] = bNode.DeepClone();
        }

        return result;
    }

    private void CompileJson()
    {
        foreach (var (identifierText, data) in definitionsData)
        {
            JsonObject json = FileInstance.OpenOrError(data.declarationFile).AsJsonNode<JsonObject>();

            foreach (string mergeFile in data.mergeFiles)
                json = MergeJson(json, FileInstance.OpenOrError(mergeFile).AsJsonNode<JsonObject>());
            
            foreach (string overwriteFile in data.overwriteFiles)
                json = FileInstance.OpenOrError(overwriteFile).AsJsonNode<JsonObject>();
            
            compiledJsonData.Add((Identifier.Parse(identifierText), json, data.declarationFile));
        }
    }

    private void LoadDefinitions()
    {
        // yooooo c# tuples are so cool
        foreach (var (identifier, compiledJson, declarationFilePath) in compiledJsonData)
        {
            // stuck on .NET 8.0 so I cant just JsonExtension.Load()
            // look for `public static LoadFile()` in JsonExtension
            var loadMethod = typeof(JsonExtension).GetMethod("Load", System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                ?? throw new JsonExtensionLoadMethodNotFound(this);
            
            ResourceType newItem = (ResourceType)loadMethod.Invoke(null, [compiledJson, identifier.ToString()]);
            newItem.FilePath = declarationFilePath;

            Game.RegistryHolder.AddToRegistry(includeNamespace ? identifier : new(identifier.Path), newItem);
        }
    }

    public void LoadNamespace(ModResource mod, string namespaceID)
    {
        FolderInstance registryFolder = FolderInstance.Open(mod.DirectoryPath.PathJoin(registryFolderJoinedPath).PathJoin(namespaceID));
        if (registryFolder == null)
            return;
        
        DiscoverFilesRecursive(registryFolder, namespaceID);
    }

    public void StoppedLoadingMods()
    {
        CompileJson();
        LoadDefinitions();

        // clear data for loading more than once
        definitionsData = [];
        compiledJsonData = [];
    }

    public class JsonExtensionLoadMethodNotFound(RegistryPopulatorModExtension<ResourceType, JsonExtension> causingExtension) : Exception($"{causingExtension.GetType().Name} ran into issue: public static method {typeof(JsonExtension).Name}.Load was not found") { public readonly RegistryPopulatorModExtension<ResourceType, JsonExtension> CausingExtension = causingExtension; }
}