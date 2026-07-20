using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace BlasII.CustomSkins;

internal class SkinImporter
{
    // Will return a list of full skin data in the future
    public void LoadAllSkins(string directory)
    {
        Directory.CreateDirectory(directory);

        ModLog.Warn("Found folders");
        foreach (string path in Directory.GetDirectories(directory))
        {
            try
            {
                LoadSkin(path);
            }
            catch (System.Exception ex)
            {
                ModLog.Error($"Failed to load skin from {path} ({ex.Message})");
            }
        }
    }

    private void LoadSkin(string directory)
    {
        string infoPath = Path.Combine(directory, "info.json");
        string texturePath = Path.Combine(directory, "texture.png");

        if (!File.Exists(infoPath))
            throw new System.Exception("No info file was present");

        if (!File.Exists(texturePath))
            throw new System.Exception("No texture file was present");

        var info = LoadInfo(infoPath);
        var texture = LoadTexture(texturePath);

        ModLog.Warn($"{info.Id} {info.Name} by {info.Author}");
    }

    private SkinInfo LoadInfo(string path)
    {
        string text = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<SkinInfo>(text);
    }

    private Texture2D LoadTexture(string path)
    {
        return null;
    }
}
