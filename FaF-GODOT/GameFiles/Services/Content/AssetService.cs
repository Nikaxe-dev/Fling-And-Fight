using System;
using System.Collections.Generic;
using FaF.Debug;
using FaF.Services.Content.Resources;
using Godot;

namespace FaF.Services.Content;

public partial class AssetService() : Service([])
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Services/AssetService");

    #region State

    // example: {"new_sedes:world/new_sedes_godot": "res://Packs/faf/mods/new_sedes/assets/world/new_sedes_godot.tscn"}
    public readonly Dictionary<string, string> FileMappings = [];

    #endregion

    #region Helper Methods

    #endregion

    #region Asset Methods

    public string GetAssetPath(string fullAssetID)
    {
        if (FileMappings.TryGetValue(fullAssetID, out string assetPath))
            return assetPath;
        else
            throw new AssetNotFoundException(fullAssetID);
    }

    public void AddAssetPath(string fullAssetID, string absoluteResourcePath)
        => FileMappings.Add(fullAssetID, absoluteResourcePath);

    public T GetAsset<T>(string fullAssetID) where T : Resource
    {
        string assetPath = GetAssetPath(fullAssetID);
        if (ResourceLoader.Exists(assetPath))
        {
            T loadedResource = ResourceLoader.Load<T>(assetPath)
                ?? throw new AssetFileIsWrongTypeException(fullAssetID, assetPath, typeof(T));
            
            return loadedResource;
        } else
            throw new AssetFileNotFoundException(fullAssetID, assetPath);
    }

    #endregion

    #region Exceptions

    public class AssetNotFoundException(string fullAssetID) : Exception($"Asset '{fullAssetID}' failed to load: the path to this asset was not registered.");
    public class AssetFileNotFoundException(string fullAssetID, string givenPath) : Exception($"Asset '{fullAssetID}' failed to load: the file given at '{givenPath}' does not exist.");
    public class AssetFileIsWrongTypeException(string fullAssetID, string givenPath, Type resourceTypeExpected) : Exception($"Asset '{fullAssetID} failed to load: the file given at '{givenPath}' does not match the expected resource type of '{resourceTypeExpected.Name}''");

    #endregion
}