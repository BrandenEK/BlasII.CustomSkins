using BlasII.CheatConsole;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins;

internal class SkinCommand : ModCommand
{
    public SkinCommand() : base("skin") { }

    public override void Execute(string[] args)
    {
        var sprites = Resources.FindObjectsOfTypeAll<Sprite>()
                .Where(x => x.name.StartsWith("TPO"))
                .DistinctBy(x => x.name)
                .OrderBy(x => x.name)
                .ToDictionary(x => x.name, x => x);

        var exporter = new Exporter(Main.CustomSkins.FileHandler.ContentFolder);
        exporter.ExportAll(sprites);
    }
}
