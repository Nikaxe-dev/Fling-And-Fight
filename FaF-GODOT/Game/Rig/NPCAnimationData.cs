using Godot;

namespace FaF.Game.Rig;

[GlobalClass]
public partial class NPCAnimationData : Resource
{
    [Export] public required string ANIMATION_ID = "HumanoidRig6Animations/fall";
    [Export] public required float PLAYBACK_SPEED = 1.0f;
    [Export] public required float PLAYBACK_BLEND = 0.2f;
}