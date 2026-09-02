namespace FaF.Editor.Attributes;

# nullable enable

/// <summary>
/// Tells the editor to save the property in the world.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Property, Inherited = false)]
public sealed class SaveAttribute(string? KeyName = null) : System.Attribute
{
    public readonly string? KeyName = KeyName;
}