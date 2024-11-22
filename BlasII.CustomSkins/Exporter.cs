using BlasII.ModdingAPI;
using System.Collections.Generic;
using UnityEngine;

namespace BlasII.CustomSkins;

/// <summary>
/// Handles exporting all found sprites
/// </summary>
internal class Exporter(string path)
{
    private readonly string _exportFolder = path;

    public void ExportAll(Dictionary<string, Sprite> export)
    {
        ModLog.Warn("Starting Export...");

    }
}
