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

    private void Replace(string folder)
    {
        folder = Path.Combine(Main.CustomSkins.FileHandler.ModdingFolder, "skins", folder);
        Main.CustomSkins.StartImport(folder, Main.CustomSkins.ReplaceSkin);
    }

    private void Merge(string folder)
    {
        folder = Path.Combine(Main.CustomSkins.FileHandler.ModdingFolder, "skins", folder);
        Main.CustomSkins.StartImport(folder, Main.CustomSkins.MergeSkin);
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
