namespace FaF.Editor.Attributes;

[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class EditorAccessAttribute(string DisplayName, bool ReadOnly = false) : System.Attribute
{
    public string DisplayName = DisplayName;
    public bool ReadOnly = ReadOnly;
}