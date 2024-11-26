using BlasII.ModdingAPI;
using System.Collections;
using System.Linq;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Just lists the spritesheet names instead of exporting
/// </summary>
public class FakeExporter : IExporter
{
    /// <inheritdoc/>
    public IEnumerator ExportAll(SpriteCollection sprites, string directory)
    {
        ModLog.Warn("Starting Export...");

        // Group sprites by name
        var groups = sprites.GroupBy(x => x.Key[0..x.Key.LastIndexOf('_')]);

        foreach (var group in groups)
        {
            ModLog.Info($"Exporting {group.Key}");
            yield return null;
        }

        ModLog.Warn("Finished export");
    }
}
