﻿using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using Newtonsoft.Json;
using System.Collections;
using System.IO;
using UnityEngine;

namespace BlasII.CustomSkins.Importers;

/// <summary>
/// Handles importing all custom sprites
/// </summary>
public class SimpleImporter : IImporter
{
    /// <inheritdoc/>
    public SpriteCollection Result { get; private set; }

    private readonly int _importsPerFrame;
    private int _currentImports;

    /// <summary>
    /// Creates a new IImporter
    /// </summary>
    public SimpleImporter(int importsPerFrame)
    {
        _importsPerFrame = importsPerFrame;
    }

    /// <inheritdoc/>
    public IEnumerator ImportAll(string directory)
    {
        // Create output dictionary
        Result = [];
        _currentImports = 0;

        // Get all json files in the import folder
        foreach (var file in Directory.GetFiles(directory, "*.json", SearchOption.TopDirectoryOnly))
        {
            yield return Import(Path.GetFileNameWithoutExtension(file), directory);
        }
    }

    private IEnumerator Import(string animation, string directory)
    {
        ModLog.Info($"Importing {animation}");

        // Import info list from import folder
        var json = File.ReadAllText(Path.Combine(directory, $"{animation}.json"));
        var infos = JsonConvert.DeserializeObject<SpriteInfo[]>(json);

        // Import texture from import folder
        var bytes = File.ReadAllBytes(Path.Combine(directory, $"{animation}.png"));
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.LoadImage(bytes, false);
        texture.filterMode = FilterMode.Point;

        // Load each sprite from the texture based on its info
        foreach (var info in infos)
        {
            if (Result.ContainsKey(info.Name))
                ModLog.Warn($"{info.Name} was already imported");

            var rect = new Rect(info.Position, info.Size);
            var sprite = Sprite.Create(texture, rect, info.Pivot, info.PixelsPerUnit, 0, SpriteMeshType.FullRect);
            sprite.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Result[info.Name] = sprite;
            if (_currentImports++ % _importsPerFrame == 0)
                yield return null;
        }
    }
}
