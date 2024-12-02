using BlasII.CheatConsole;
using BlasII.ModdingAPI;
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
#if DEBUG
            case "debug":
                {
                    if (!ValidateParameterCount(args, 1))
                        return;

                    Debug();
                    break;
                }
#endif
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
        string folder = Main.CustomSkins.FileHandler.ContentFolder;
        Main.CustomSkins.StartExport(folder);
    }

    private void Debug()
    {
        ModLog.Warn("Running debug command");

        var sprites = Resources.FindObjectsOfTypeAll<Sprite>()
            .Select(x => x.name)
            .Where(x => !string.IsNullOrEmpty(x))
            .DistinctBy(x => x)
            .OrderBy(x => x);

        foreach (string name in sprites)
            ModLog.Info(name);
    }
}
