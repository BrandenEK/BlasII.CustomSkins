using BlasII.ModdingAPI;
using HarmonyLib;
using Il2Cpp;
using Il2CppTGK.Game;
using Il2CppTGK.Game.Components.Animation;
using Il2CppTGK.Game.Components.UI;
using Il2CppTGK.Game.Managers.Config;
using UnityEngine;
using UnityEngine.Playables;
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

        foreach (var recolor in __instance.skinsListUI)
            recolor.UpdateRecolor();

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

        foreach (var recolor in __instance.skinsListUI)
        {
            //ModLog.Info(recolor.transform.parent.name + " (" + recolor.ID.name + ") applies to " + recolor.recolorImage.gameObject.GetInstanceID());
            //var anims = recolor.transform.parent.GetComponent<RandomizeAnimator>().animators;
            //for (int i = 0; i < anims.Length; i++)
            //{
            //    ModLog.Error(anims[i].name + ": " + anims[i].gameObject.GetInstanceID());
            //}

            //recolor.recolorImage = null;
            //recolor.UpdateRecolor();

            //var callbacks = recolor.transform.parent.GetComponent<MonoBehaviourCallbacks>();

            //ModLog.Info(callbacks.OnStartCallback.GetPersistentEventCount());
            //for (int i = 0; i < callbacks.OnStartCallback.GetPersistentEventCount(); i++)
            //    ModLog.Warn(callbacks.OnStartCallback.GetPersistentMethodName(i));

            //callbacks.OnEnableCallback.RemoveAllListeners();
            //callbacks.OnStartCallback.RemoveAllListeners();
            //callbacks.OnDestroyCallback.RemoveAllListeners();
            //callbacks.OnDisableCallback.RemoveAllListeners();

            //ModLog.Info(recolor.transform.parent.GetChild(0).GetComponent<Image>().sprite.name ?? "none");
            //ModLog.Warn(recolor.transform.parent.GetChild(1).GetComponent<Image>().sprite.name ?? "none");
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

[HarmonyPatch(typeof(SkinSelectorLogic), nameof(SkinSelectorLogic.SetCurrentElement))]
class SkinSelectorLogic_SetCurrentElement_Patch
{
    public static void Postfix(SkinSelectorLogic __instance)
    {
        ModLog.Error("Forcing recolor");
        var recolor = __instance.skinsListUI[__instance.currentSkinIndex];
        //recolor.ID = Main.CustomSkins.GetSkin(0).Palette;
        //recolor.recolorImage = __instance.skinsListUI[1].transform.parent.GetChild(1).GetComponent<Image>();
        //recolor.UpdateRecolor();

        //return;

        if (__instance.currentSkinIndex < 8)
            return;

        var skin = Main.CustomSkins.GetSkin(__instance.currentSkinIndex - 8);
        __instance.skinName.SetText($"{skin.Info.Name} <color=#FFDE92>by</color> {skin.Info.Author}");
    }
}
