using BlasII.CheatConsole;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins;

internal class SkinCommand : ModCommand
{
    public SkinCommand() : base("skin") { }

    public override void Execute(string[] args)
    {
        switch (args[0])
        {
            case "export":
                {
                    if (!ValidateParameterCount(args, 1))
                        return;

                    Export();
                    break;
                }
            default:
                {
                    WriteFailure("Unknown subcommand: " + args[0]);
                    break;
                }
        }
    }

    private void Export()
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
