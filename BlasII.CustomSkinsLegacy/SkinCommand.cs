using BlasII.CheatConsole.Attributes;
using BlasII.CheatConsole.Commands;
using BlasII.CustomSkins.Extensions;
using BlasII.ModdingAPI;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BlasII.CustomSkins;

internal class SkinCommand : ModComplexCommand
{
    public SkinCommand() : base("cskin") { }

    [SubCommand]
    private void List()
    {
        string folder = Path.Combine(Main.CustomSkins.FileHandler.ModdingFolder, "skins");

        var skins = Directory.Exists(folder)
            ? Directory.GetDirectories(folder).Select(Path.GetFileName)
            : [];

        var sb = new StringBuilder();
        sb.AppendLine("Installed skins:");

        if (skins.Any())
        {
            foreach (var skin in skins)
                sb.AppendLine(skin);
        }
        else
        {
            sb.AppendLine("None");
        }

        Write(sb.ToString());
    }

    [SubCommand]
    private void Set(string id)
    {
        string folder = Path.Combine(Main.CustomSkins.FileHandler.ModdingFolder, "skins", id, "textures");

        if (Directory.Exists(folder))
        {
            Write($"Setting selected skin to {id}");
            Main.CustomSkins.CurrentSkin = id;
        }

        Main.CustomSkins.StartImport(folder, Main.CustomSkins.ReplaceSkin);
    }

    [SubCommand]
    private void Reset()
    {
        Write($"Restting selected skin to default");
        Main.CustomSkins.CurrentSkin = string.Empty;

        Main.CustomSkins.ResetSkin();
    }

    [SubCommand]
    private void Merge(string id)
    {
        string folder = Path.Combine(Main.CustomSkins.FileHandler.ModdingFolder, "skins", id, "textures");

        if (Directory.Exists(folder))
        {
            Write($"Merging selected skin with {id}");
            // Add merge to current id
        }

        Main.CustomSkins.StartImport(folder, Main.CustomSkins.MergeSkin);
    }

    [SubCommand]
    private void Export(string type)
    {
        string folder = Main.CustomSkins.FileHandler.ContentFolder;
        Main.CustomSkins.StartExport(type, folder);
    }

#if DEBUG
    [SubCommand]
    private void Debug()
    {
        ModLog.Warn("Running debug command");

        var loadedSprites = Resources.FindObjectsOfTypeAll<Sprite>()
            .Where(x => !string.IsNullOrEmpty(x.name))
            .Select(x => x.GetUniqueName())
            .Distinct()
            .OrderBy(x => x);

        var loadedTextures = Resources.FindObjectsOfTypeAll<Texture2D>()
            .Where(x => !string.IsNullOrEmpty(x.name))
            .Select(x => x.name)
            .Distinct()
            .OrderBy(x => x);

        var visibleSprites = Object.FindObjectsOfType<SpriteRenderer>()
            .Where(x => x.sprite != null && !string.IsNullOrEmpty(x.sprite.name))
            .Select(x => x.sprite.GetUniqueName())
            .Distinct()
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
#endif    
}
