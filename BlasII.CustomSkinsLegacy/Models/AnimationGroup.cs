
namespace BlasII.CustomSkins.Models;

/// <summary>
/// A group of related animations
/// </summary>
public class AnimationGroup
{
    /// <summary>
    /// The name of the group
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// The names of the animations in the group
    /// </summary>
    public string[] Animations { get; set; }
}
