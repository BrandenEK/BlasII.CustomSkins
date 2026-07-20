
namespace BlasII.CustomSkins;

/// <summary>
/// Properties relating to the import/export process
/// </summary>
public class SkinConfig
{
    /// <summary>
    /// Whether all only player sprites should be replaced instead of all objects
    /// </summary>
    public bool UsePerformanceMode { get; set; } = false;

    /// <summary>
    /// How many sprites should be imported in a single frame
    /// </summary>
    public int ImportsPerFrame { get; set; } = 30;

    /// <summary>
    /// How many pixels wide the animation texture should be until it wraps
    /// </summary>
    public int ExportAnimationWidth { get; set; } = 2048;

    /// <summary>
    /// How many pixels tall the group texture should be until it wraps
    /// </summary>
    public int ExportGroupHeight { get; set; } = 8192;
}
