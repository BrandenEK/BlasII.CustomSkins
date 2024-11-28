using System.Collections;
using BlasII.CustomSkins.Models;

namespace BlasII.CustomSkins.Importers;

/// <summary>
/// Handles importing all present spritesheets
/// </summary>
public interface IImporter
{
    /// <summary>
    /// The sprites that were imported
    /// </summary>
    public SpriteCollection Result { get; }

    /// <summary>
    /// Imports all spritesheets in the directory
    /// </summary>
    public IEnumerator ImportAll(string directory);
}
