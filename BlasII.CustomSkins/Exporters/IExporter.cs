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
    public void ExportAll(Dictionary<string, Sprite> export, string directory);
}
