namespace GodotShell.Runtime.Values;

public abstract class GDSValue
{
    public virtual string AsTextReply()
        => ToString();
}