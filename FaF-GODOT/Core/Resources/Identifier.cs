using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace FaF.Core.Resources;

public struct Identifier
{
    public string Namespace;
    public string Path;

    public static bool TryParse(string identifierText, out Identifier identifier, out Exception issue)
    {
        identifier = default;

        if (identifierText == "")
        {
            issue = new IdentifierIsEmptyException();
            return false;
        }

        if (identifierText.EndsWith(':'))
        {
            issue = new IdentifierPathNotSetException(identifierText);
            return false;
        }

        string[] keys = identifierText.Split(":");

        if (keys.Length > 2)
        {
            issue = new IdentifierHasTooManyKeysException(identifierText);
            return false;
        }

        // new_sedes = { Namespace: null, Path: 'new_sedes' }
        // faf:grabline = { Namespace: 'faf', Path: 'grabline' }
        
        if (keys.Length == 1)
            identifier = new(keys[0]);
        else
            identifier = new(keys[0], keys[1]);

        issue = null;
        return true;
    }

    public static bool TryParse(string identifierText, out Identifier identifier)
        => TryParse(identifierText, out identifier, out _);
    
    public static Identifier Parse(string identifierText)
    {
        if (TryParse(identifierText, out Identifier identifier, out Exception issue))
            return identifier;
        else
            throw issue;
    }

    public override readonly string ToString()
        => $"{(Namespace == null ? "" : $"{Namespace}:")}{Path}";

    public override readonly bool Equals([NotNullWhen(true)] object obj)
        => obj != null && obj is Identifier identifier && identifier.ToString() == ToString();

    public override readonly int GetHashCode()
        => HashCode.Combine(Namespace, Path);

    public Identifier(string path)
        => Path = path;
    
    public Identifier(string @namespace, string path)
    {
        Path = path;
        Namespace = @namespace;
    }

    public class IdentifierIsEmptyException() : Exception($"The given identifier was an empty string.");
    public class IdentifierPathNotSetException(string identifier) : Exception($"Expected identifier path after colon in {identifier}");
    public class IdentifierHasTooManyKeysException(string identifier) : Exception($"Identifier string {identifier} has too many keys. There should only be an optional namespace & path in any identifier.");
}