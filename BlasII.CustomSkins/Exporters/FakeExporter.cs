using BlasII.ModdingAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Just lists the spritesheet names instead of exporting
/// </summary>
public class FakeExporter : IExporter
{
    /// <inheritdoc/>
    public void ExportAll(Dictionary<string, Sprite> export, string directory)
    {
        ModLog.Warn("Starting Export...");

        // Group sprites by name
        var groups = export.GroupBy(x => x.Key[0..x.Key.LastIndexOf('_')]);

        foreach (var group in groups)
        {
            ModLog.Info($"Exporting {group.Key}");
        }

        ModLog.Warn("Finished export");
    }
}
