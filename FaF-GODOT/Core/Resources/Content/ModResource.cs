using System.Collections.Generic;
using FaF.Core.Lib;

namespace FaF.Core.Resources.Content;

public partial class ModResource : ItemLikeResource
{
    public Dictionary<string, string> Links = [];

    public Version Version = new();
    
    public string[] DefinedNamespaces = [];
    public Dictionary<string, string> Dependencies = [];

    public bool _DISABLED = false;
}