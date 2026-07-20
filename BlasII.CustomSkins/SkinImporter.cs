using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using Il2CppTGK.Game;
using Il2CppTGK.Game.Managers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlasII.CustomSkins;

internal class SkinImporter
{
    public List<SkinData> LoadAllSkins(string directory)
    {
        Directory.CreateDirectory(directory);

        var skins = new List<SkinData>();

        foreach (string path in Directory.GetDirectories(directory))
        {
            try
            {
                skins.Add(LoadSkin(path));
            }
            catch (System.Exception ex)
            {
                ModLog.Error($"Failed to load skin from {path} ({ex.Message})");
            }
        }

        return skins;
    }

    private SkinData LoadSkin(string directory)
    {
        string infoPath = Path.Combine(directory, "info.json");
        string texturePath = Path.Combine(directory, "texture.png");

        if (!File.Exists(infoPath))
            throw new System.Exception("No info file was present");

        if (!File.Exists(texturePath))
            throw new System.Exception("No texture file was present");

        return RegisterPalette(LoadInfo(infoPath), LoadTexture(texturePath));
    }

    private SkinInfo LoadInfo(string path)
    {
        string text = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<SkinInfo>(text);
    }

    private Texture2D LoadTexture(string path)
    {
        var bytes = File.ReadAllBytes(path);

        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes, false);
        tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
        tex.filterMode = FilterMode.Point;

        return tex;
    }

    private SkinData RegisterPalette(SkinInfo info, Texture2D texture)
    {
        var palette = ScriptableObject.CreateInstance<PaletteID>();
        palette.name = info.Id;
        palette.id = info.Id.GetHashCode();
        palette.hideFlags = HideFlags.DontUnloadUnusedAsset;

        CoreCache.PlayerRecolorManager.config.palettes.Add(new Il2CppTGK.Game.Managers.Config.PlayerRecolorManagerConfig.PaletteEntry()
        {
            palette = texture,
            ID = palette
        });

        ModLog.Info($"Loaded custom skin {info.Id} ({info.Name})");
        return new SkinData(info, texture, palette);
    }
}
