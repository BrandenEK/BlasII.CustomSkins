using System.Collections.Generic;
using BlasII.CustomSkins.Models;
using UnityEngine;

namespace BlasII.CustomSkins.Extensions;

internal static class DictionaryExtensions
{
    public static SpriteCollection AsCollection(this Dictionary<string, Sprite> sprites)
    {
        var collection = new SpriteCollection();

        foreach (var kvp in sprites)
            collection.Add(kvp.Key, kvp.Value);

        return collection;
    }
}
