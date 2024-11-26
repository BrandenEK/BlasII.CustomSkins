using BlasII.CheatConsole;
using BlasII.CustomSkins.Extensions;
using System.IO;
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
            case "import":
                {
                    if(!ValidateParameterCount(args, 2))
                        return;

                    Import(args[1]);
                    break;
                }
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

    private void Import(string folder)
    {
        string path = Path.Combine(Main.CustomSkins.FileHandler.ModdingFolder, "skins", folder);

        if (!Directory.Exists(path))
        {
            WriteFailure($"{path} does not exist.");
            return;
        }

        var spritesheets = Main.CustomSkins.Importer.ImportAll(path);
        Main.CustomSkins.UpdateSkin(spritesheets);
    }

    private void Export()
    {
        var sprites = Resources.FindObjectsOfTypeAll<Sprite>()
            .Where(x => x.name.StartsWith("TPO"))
            .DistinctBy(x => x.name)
            .OrderBy(x => x.name)
            .ToDictionary(x => x.name, x => x)
            .AsCollection();

        Main.CustomSkins.Exporter.ExportAll(sprites, Main.CustomSkins.FileHandler.ContentFolder);
    }
}
