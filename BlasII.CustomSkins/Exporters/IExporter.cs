using System.Collections;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Handles exporting all given spritesheets
/// </summary>
public interface IExporter
{
    /// <summary>
    /// Exports all spritesheets into the directory
    /// </summary>
    public IEnumerator ExportAll(SpriteCollection sprites, string directory);
}
