using BlasII.ModdingAPI;
using HarmonyLib;
using Il2CppTGK.Game.Components;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins;

[HarmonyPatch(typeof(UpdateOriginalRecolorSprite), nameof(UpdateOriginalRecolorSprite.UpdateSprite))]
class UpdateOriginalRecolorSprite_UpdateSprite_Patch
{
    public static void Postfix(UpdateOriginalRecolorSprite __instance)
    {
        if (__instance.name != "armor_indexed")
            return;

        if (__instance.dest == null)
            return;

        if (_normal == null || _indexed == null)
            CacheMaterials();

        __instance.dest.material = _normal;
    }

    private static void CacheMaterials()
    {
        var mats = Resources.FindObjectsOfTypeAll<Material>();

        _normal = mats.FirstOrDefault(x => x.name == "Player");
        _indexed = mats.FirstOrDefault(x => x.name == "Player Indexed");

        if (_normal == null || _indexed == null)
            ModLog.Error("Unable to find player materials!");
        else
            ModLog.Info("Successfully cached player materials");
    }

    private static Material _normal;
    private static Material _indexed;
}
