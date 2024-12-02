using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Handles exporting all given spritesheets
/// </summary>
public interface IExporter
{
    /// <summary>
    /// Exports all spritesheets into the directory
    /// </summary>
    public IEnumerator ExportAll(IEnumerable<Sprite> sprites, string directory);
}
