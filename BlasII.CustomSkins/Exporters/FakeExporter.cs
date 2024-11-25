using BlasII.ModdingAPI;
using MelonLoader;
using System.Collections;
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
        MelonCoroutines.Start(ExportCoroutine(export, directory));
    }

    private IEnumerator ExportCoroutine(Dictionary<string, Sprite> export, string directory)
    {
        // Group sprites by name
        var groups = export.GroupBy(x => x.Key[0..x.Key.LastIndexOf('_')]);

        // Export each individual spritesheet
        foreach (var group in groups)
        {
            var sprites = group
                .OrderBy(x => int.Parse(x.Key[(x.Key.LastIndexOf('_') + 1)..]))
                .Select(x => x.Value);

            ModLog.Info($"Exporting {group.Key}");
            yield return null;
        }

        ModLog.Warn("Finished export");
    }
}
