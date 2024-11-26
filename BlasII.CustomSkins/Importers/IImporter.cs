using System.Collections.Generic;
using UnityEngine;

namespace BlasII.CustomSkins.Importers;

/// <summary>
/// Handles importing all present spritesheets
/// </summary>
public interface IImporter
{
    /// <summary>
    /// Imports all spritesheets in the directory
    /// </summary>
    public Dictionary<string, Sprite> ImportAll(string directory);
}
