
namespace BlasII.CustomSkins;

/// <summary>
/// Models a <see cref="UnityEngine.Sprite"/>
/// </summary>
public class SpriteInfo
{
    /// <summary>
    /// The name of the sprite
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The ppu of the sprite
    /// </summary>
    public int PixelsPerUnit { get; init; }

    /// <summary>
    /// The x position of the sprite in pixels
    /// </summary>
    public int Position { get; init; }

    /// <summary>
    /// The width of the sprite in pixels
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// The height of the sprite in pixels
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// The normalized x pivot of the sprite
    /// </summary>
    public float Pivot { get; init; }
}
