using BlasII.ModdingAPI;
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
    public IEnumerator ExportAll(IEnumerable<Sprite> sprites, string directory)
    {
        // Group sprites by name
        var groups = sprites.GroupBy(x => x.name[0..x.name.LastIndexOf('_')]);

        foreach (var group in groups)
        {
            ModLog.Info($"Exporting {group.Key}");
            yield return null;
        }
    }
}
