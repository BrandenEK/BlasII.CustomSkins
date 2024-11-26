using BlasII.CheatConsole;
using BlasII.CustomSkins.Finders;
using System.IO;

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
        IFinder finder = new FinderWithCrisanta(new ResourcesFinder());
        Main.CustomSkins.Exporter.ExportAll(finder, Main.CustomSkins.FileHandler.ContentFolder);
    }
}
