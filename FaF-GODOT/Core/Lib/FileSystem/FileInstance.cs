using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;

namespace FaF.Core.Lib.FileSystem;

public class FileInstance : FSInstance
{
    public static readonly JsonDocumentOptions DefaultJsonParsingOptions = new() {CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true};

    public override string Name => Path.GetFile().Split('.')[0];
    public string FullName => Path.GetFile();
    public string BaseName => Path.GetBaseName();

    public static new FileInstance Open(string path)
    {
        if (FileAccess.FileExists(path))
            return new FileInstance() {Path = path};
        else
            return null;
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

    public string ReadText()
    {
        string text = FileAccess.GetFileAsString(Path);
        if (text == "")
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

    public bool IsGodotImportExtension() => GetExtension() == "import" || GetExtension() == "uid";

    public class FileAccessGetFileAsStringFailedException(FileInstance file, Error errorCode) : Exception($"Failed to read file '{file}' as text with error code: {errorCode}");
    public class FileReadJsonFailedException(FileInstance file, Exception exception) : Exception($"Failed to read file '{file}' as json with message: {exception.Message}");
}