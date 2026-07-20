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
        ModLog.Warn("Setting initial skins");

        //ModLog.Info("Entries in the skins list");
        //foreach (var entry in __instance.skins)
        //{
        //    ModLog.Warn(entry.nameTerm + ": " + entry.isUnlocked);
        //}

        //ModLog.Info("Entries in the skins UI list");
        //foreach (var entry in __instance.skinsListUI)
        //{
        //    ModLog.Warn(entry.gameObject.transform.parent.name);
        //}

        if (_createdUI)
            return;
        _createdUI = true;

        var skinconfig = __instance.skinConfig;
        var root = __instance.skinsRoot;
        var prefab = root.GetChild(0);

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
            recolor.recolorImage = recolor.transform.parent.GetChild(1).GetComponent<Image>();
            recolor.UpdateRecolor();

            // Add the element to the UI lists
            __instance.skinsListUI.Add(recolor);
            __instance.skins.Add(new SkinSelectorLogic.SkinElement()
            {
                recolor = recolor,
                nameTerm = string.Empty,
                isUnlocked = true,
            });

            ModLog.Warn("Added new skin element to UI");

        }

        //ModLog.Info("Entries in the palette config");
        //for (int i = 0; i < config.palettes.Length; i++)
        //{
        //    var entry = config.palettes[i];
        //    ModLog.Warn(entry.ID.name + ": " + entry.term + ", " + entry.UIImage?.name);
        //}
    }

    private static bool _createdUI = false;
}
