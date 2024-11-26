
namespace BlasII.CustomSkins.Importers;

/// <summary>
/// Handles importing all present spritesheets
/// </summary>
public interface IImporter
{
    /// <summary>
    /// Imports all spritesheets in the directory
    /// </summary>
    public SpriteCollection ImportAll(string directory);
}
