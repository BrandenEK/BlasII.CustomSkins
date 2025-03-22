using BlasII.CheatConsole;
using BlasII.CustomSkins.Extensions;
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
            case "set":
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
            case "merge":
                {
                    if (!ValidateParameterCount(args, 2))
                        return;

                    Merge(args[1]);
                    break;
                }
            case "export":
                {
                    if (!ValidateParameterCount(args, 2))
                        return;

                    Export(args[1]);
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

    private void Export(string type)
    {
        string folder = Main.CustomSkins.FileHandler.ContentFolder;
        Main.CustomSkins.StartExport(type, folder);
    }

    private void Debug()
    {
        ModLog.Warn("Running debug command");

        var loadedSprites = Resources.FindObjectsOfTypeAll<Sprite>()
            .Where(x => !string.IsNullOrEmpty(x.name))
            .Select(x => x.GetUniqueName())
            .OrderBy(x => x);

        var loadedTextures = Resources.FindObjectsOfTypeAll<Texture2D>()
            .Where(x => !string.IsNullOrEmpty(x.name))
            .Select(x => x.name)
            .OrderBy(x => x);

        var visibleSprites = Object.FindObjectsOfType<SpriteRenderer>()
            .Where(x => x.sprite != null && !string.IsNullOrEmpty(x.sprite.name))
            .Select(x => x.sprite.GetUniqueName())
            .OrderBy(x => x);

        ModLog.Error("Loaded sprites:");
        foreach (string name in loadedSprites)
            ModLog.Info(name);

        ModLog.Error("Loaded textures:");
        foreach (string name in loadedTextures)
            ModLog.Info(name);

        ModLog.Error("Visible sprites:");
        foreach (string name in visibleSprites)
            ModLog.Info(name);
    }
}
