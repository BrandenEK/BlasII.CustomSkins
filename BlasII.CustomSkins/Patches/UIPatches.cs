using BlasII.ModdingAPI;
using HarmonyLib;
using Il2CppTGK.Game;
using Il2CppTGK.Game.Components.UI;
using Il2CppTGK.Game.Managers.Config;
using UnityEngine;
using UnityEngine.UI;

namespace BlasII.CustomSkins.Patches;

[HarmonyPatch(typeof(SkinSelectorLogic), nameof(SkinSelectorLogic.SetInitialsSkins))]
class SkinSelectorLogic_SetInitialsSkins_Patch
{
    public static void Prefix(SkinSelectorLogic __instance)
    {
        ModLog.Info("Adding custom skins to UI selector");

        if (_createdUI)
            return;
        _createdUI = true;

        // Cache variables
        var skinconfig = __instance.skinConfig;
        var root = __instance.skinsRoot;
        var prefab = root.GetChild(0);

        // Modify name text
        __instance.skinName.normalText.richText = true;
        __instance.skinName.shadowText.richText = true;

        foreach (var skin in Main.CustomSkins.GetAllSkins())
        {
            // Unlock the skin
            CoreCache.PlayerRecolorManager.UnlockSkin(skin.Palette);

            // Add data to the UI config
            skinconfig.palettes.Add(new PlayerRecolorManagerUIConfig.UIPaletteConfig()
            {
                ID = skin.Palette,
                term = string.Empty,
                UIImage = skinconfig.palettes[0].UIImage,
                UIImageBig = skinconfig.palettes[0].UIImageBig,
            });

            // Create a new UI element
            var newElement = Object.Instantiate(prefab.gameObject, root);
            newElement.name = $"Skin {skin.Info.Id}";

            // Setup the recolor data
            var recolor = newElement.transform.GetChild(0).GetComponent<ApplyRecolor>();
            recolor.ID = skin.Palette;
            recolor.recolorImage = newElement.transform.GetChild(1).GetComponent<Image>();
            recolor.recolorImage.material = new Material(__instance.skinsListUI[0].recolorImage.material);

            // Add the element to the UI lists
            __instance.skinsListUI.Add(recolor);
            __instance.skins.Add(new SkinSelectorLogic.SkinElement()
            {
                recolor = recolor,
                nameTerm = string.Empty,
                isUnlocked = true,
            });
        }
    }

    private static bool _createdUI = false;
}

[HarmonyPatch(typeof(SkinSelectorLogic), nameof(SkinSelectorLogic.SetCurrentElement))]
class SkinSelectorLogic_SetCurrentElement_Patch
{
    public static void Postfix(SkinSelectorLogic __instance)
    {
        if (__instance.currentSkinIndex < NUM_VANILLA_SKINS)
            return;

        var skin = Main.CustomSkins.GetSkin(__instance.currentSkinIndex - NUM_VANILLA_SKINS);
        __instance.skinName.SetText($"{skin.Info.Name} <color=#FFDE92>by</color> {skin.Info.Author}");
    }

    private const int NUM_VANILLA_SKINS = 8;
}
