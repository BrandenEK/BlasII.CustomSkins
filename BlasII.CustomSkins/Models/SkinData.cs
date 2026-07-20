using Il2CppTGK.Game.Managers;
using UnityEngine;

namespace BlasII.CustomSkins.Models;

internal class SkinData(SkinInfo info, Texture2D texture, PaletteID palette)
{
    public SkinInfo Info { get; } = info;

    public Texture2D Texture { get; } = texture;

    public PaletteID Palette { get; } = palette;
}
