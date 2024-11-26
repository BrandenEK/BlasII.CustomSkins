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
            case "merge":
                {
                    if (!ValidateParameterCount(args, 2))
                        return;

                    Merge(args[1]);
                    break;
                }
            case "replace":
                {
                    if (!ValidateParameterCount(args, 2))
                        return;

                    Replace(args[1]);
                    break;
                }
            case "reset":
                {
                    if (!ValidateParameterCount(args, 1))
                        return;

                    Reset();
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

    private bool TryImport(string folder, out SpriteCollection spritesheets)
    {
        string path = Path.Combine(Main.CustomSkins.FileHandler.ModdingFolder, "skins", folder);

        if (!Directory.Exists(path))
        {
            WriteFailure($"{path} does not exist.");

            spritesheets = []; 
            return false;
        }

        spritesheets = Main.CustomSkins.Importer.ImportAll(path);
        return true;
    }

    private void Replace(string folder)
    {
        if (!TryImport(folder, out SpriteCollection spritesheets))
            return;

        Main.CustomSkins.ReplaceSkin(spritesheets);
    }

    private void Merge(string folder)
    {
        if (!TryImport(folder, out SpriteCollection spritesheets))
            return;

        Main.CustomSkins.MergeSkin(spritesheets);
    }

    private void Reset()
    {
        Main.CustomSkins.ResetSkin();
    }

    private void Export()
    {
        IFinder finder = new FinderWithCrisanta(new ResourcesFinder());
        Main.CustomSkins.Exporter.ExportAll(finder, Main.CustomSkins.FileHandler.ContentFolder);
    }
}
