using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;

namespace FaF.Core.FileSystem;

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

    public string[] GetExtensions()
    {
        List<string> dotSplit = [.. Path.GetFile().Split('.')];
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

    public string ReadText()
    {
        string text = FileAccess.GetFileAsString(Path);
        if (text == "")
            throw new FileAccessGetFileAsStringFailed(this, FileAccess.GetOpenError());
        return text;
    }

    public bool IsJsonExtension() => GetExtension() == "json" || GetExtension() == "jsonc";

    public JsonDocument ReadJson(JsonDocumentOptions? options = null)
    {
        options ??= DefaultJsonParsingOptions;
        
        try 
        {
            return JsonDocument.Parse(ReadText(), (JsonDocumentOptions)options);
        }
        catch (Exception ex)
        {
            throw new FileReadJsonFailed(this, ex);
        }
    }

    public bool IsGodotImportExtension() => GetExtension() == "import" || GetExtension() == "uid";

    public class FileAccessGetFileAsStringFailed(FileInstance file, Error errorCode) : Exception($"Failed to read file '{file}' as text with error code: {errorCode}");
    public class FileReadJsonFailed(FileInstance file, Exception exception) : Exception($"Failed to read file '{file}' as json with message: {exception.Message}");
}