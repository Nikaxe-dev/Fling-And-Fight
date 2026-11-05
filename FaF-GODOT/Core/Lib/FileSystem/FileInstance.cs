using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;

namespace FaF.Core.Lib.FileSystem;

/// <summary>
/// Represents a file in the godot file system and provides methods for reading, writing and creating files in the file system. <br/>
/// Whats neat about FaFs implementation of the file system API is that it allows accessing multi extension files (example: faf.mod.json). <br/>
/// <br/>
/// This wraps around the godot FileAccess API.
/// </summary>
public class FileInstance : FSInstance
{
    public static readonly JsonDocumentOptions DefaultJsonParsingOptions = new() {CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true};

    public override string Name => Path.GetFile().Split('.')[0];
    public string FullName => Path.GetFile();
    public string BaseName => Path.GetBaseName();

    #region Access

    public static FileInstance Touch(string path)
    {
        FileAccess.Open(path, FileAccess.ModeFlags.Write).Dispose();
        return OpenOrError(path, new FailedToTouchFileException(path, FileAccess.GetOpenError()));
    }

    public static FileInstance Open(string path, bool makeFile = false)
    {
        if (FileAccess.FileExists(path))
            return new FileInstance() {Path = path};
        else if (makeFile)
            return Touch(path);
        else
            return null;
    }


    public static (FileInstance file, bool newlyCreated) OpenOrNew(string path)
    {
        if (FileAccess.FileExists(path))
            return (new FileInstance() {Path = path}, false);
        else
            return (Touch(path), true);
    }

    public static new FileInstance OpenOrError(string path, Exception customException = null)
    {
        FileInstance result = Open(path);
        if (result != null)
            return result;
        else
            throw customException ?? new PathDoesNotExistException(path);
    }

    public static bool TryOpen(string path, out FileInstance file)
    {
        file = Open(path);
        return file != null;
    }

    public static bool TryOpenOrError(string path, out FileInstance file, Exception customException = null)
    {
        file = OpenOrError(path, customException);
        return true;
    }

    #endregion

    #region Extensions

    public string[] GetExtensions()
    {
        List<string> dotSplit = [.. FullName.Split('.')];
        dotSplit.RemoveAt(0);
        return [.. dotSplit];
    }

    public string GetExtension(int? index = null)
    {
        var extensions = GetExtensions();

        if (index == null)
            return extensions.Last();
        else
            return extensions[(int)index];
    }

    public bool HasExtension(string extension)
        => GetExtensions().Contains(extension);
    
    public bool IsGodotImportExtension() => GetExtension() == "import" || GetExtension() == "uid";
    
    #endregion
    
    #region Reading

    public string ReadText()
    {
        string text = FileAccess.GetFileAsString(Path);
        if (FileAccess.GetOpenError() != Error.Ok)
            throw new FileAccessGetFileAsStringFailedException(this, FileAccess.GetOpenError());
        return text;
    }

    public bool IsJsonExtension() => GetExtension() == "json" || GetExtension() == "jsonc";

    public JsonDocument AsJsonDocument(JsonDocumentOptions? options = null)
    {
        options ??= DefaultJsonParsingOptions;
        
        try 
        {
            return JsonDocument.Parse(ReadText(), (JsonDocumentOptions)options);
        }
        catch (Exception ex)
        {
            throw new FileReadJsonFailedException(this, ex);
        }
    }

    public JsonNode AsJsonNode(JsonDocumentOptions? options = null)
    {
        options ??= DefaultJsonParsingOptions;

        try
        {
            return JsonNode.Parse(ReadText(), documentOptions: (JsonDocumentOptions)options);
        } catch (Exception ex)
        {
            throw new FileReadJsonFailedException(this, ex);
        }
    }

    public T AsJsonNode<T>(JsonDocumentOptions? options = null)
        where T : JsonNode
    {
        options ??= DefaultJsonParsingOptions;

        try
        {
            return (T)JsonNode.Parse(ReadText(), documentOptions: (JsonDocumentOptions)options);
        } catch (Exception ex)
        {
            throw new FileReadJsonFailedException(this, ex);
        }
    }

    #endregion

    #region Writing

    public void WriteText(string text)
    {
        FileAccess fileAccess = FileAccess.Open(Path, FileAccess.ModeFlags.Write);
        fileAccess.StoreString(text);

        fileAccess.Close();
        fileAccess.Dispose();
    }

    public void WriteJson(JsonObject json)
        => WriteText(json.ToJsonString());
    
    public void WriteJson(JsonElement json)
        => WriteText(json.GetRawText());

    #endregion

    // TODO: add members to every exception in the program like below

    public class FileAccessGetFileAsStringFailedException(FileInstance file, Error errorCode) : Exception($"Failed to read file '{file}' as text with error code: {errorCode}") { public readonly string Path = file.Path; public readonly Error ErrorCode = errorCode; }
    public class FileReadJsonFailedException(FileInstance file, Exception exception) : Exception($"Failed to read file '{file}' as json with message: {exception.Message}") { public readonly string Path = file.Path; public readonly Exception Exception = exception; }
    public class FailedToTouchFileException(string path, Error errorCode) : Exception($"Failed to touch (create) file at path '{path}' with error code: {errorCode}") { public readonly string Path = path; public readonly Error ErrorCode = errorCode; }
}