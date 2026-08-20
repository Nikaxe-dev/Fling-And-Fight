using Godot;
using System;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class SongRegistry : Registry
{
    [Export] public required AudioStream Audio;
}
