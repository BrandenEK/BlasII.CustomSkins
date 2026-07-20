using BlasII.ModdingAPI;
using HarmonyLib;
using Il2CppTGK.Game;
using Il2CppTGK.Game.Components.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BlasII.CustomSkins;

[HarmonyPatch(typeof(SkinSelectorLogic), nameof(SkinSelectorLogic.SetInitialsSkins))]
class t
{
    public static void Prefix(SkinSelectorLogic __instance)
    {
        ModLog.Warn("Setting initial skins");

        ModLog.Info("Entries in the skins list");
        foreach (var entry in __instance.skins)
        {
            ModLog.Warn(entry.nameTerm + ": " + entry.isUnlocked);
        }

        ModLog.Info("Entries in the skins UI list");
        foreach (var entry in __instance.skinsListUI)
        {
            ModLog.Warn(entry.gameObject.transform.parent.name);
        }

        ModLog.Info("Is unlocked: " + CoreCache.PlayerRecolorManager.IsSkinUnlocked(Main.CustomSkins.TestPalette));
        CoreCache.PlayerRecolorManager.UnlockSkin(Main.CustomSkins.TestPalette);

        if (done)
            return;
        done = true;

        var config = __instance.skinConfig;

        ModLog.Info("Entries in the palette config");
        for (int i = 0; i < config.palettes.Length; i++)
        {
            var entry = config.palettes[i];
            ModLog.Warn(entry.ID.name + ": " + entry.term + ", " + entry.UIImage?.name);
        }

        var root = __instance.skinsRoot;
        var prefab = root.GetChild(0);

        var newElement = Object.Instantiate(prefab.gameObject, root);
        newElement.name = "Skin Test";

        var recolor = newElement.transform.GetChild(0).GetComponent<ApplyRecolor>();

        //ModLog.Error(__instance.transform.DisplayHierarchy(99, true));
        if (recolor == null)
        {
            ModLog.Fatal("recolor is null");
            return;
        }

        recolor.ID = Main.CustomSkins.TestPalette;
        recolor.recolorImage = recolor.transform.parent.GetChild(1).GetComponent<Image>();
        recolor.UpdateRecolor();

        __instance.skinsListUI.Add(recolor);
        __instance.skins.Add(new SkinSelectorLogic.SkinElement()
        {
            recolor = recolor,
            nameTerm = "New Palette Name",
            isUnlocked = true
        });

        ModLog.Warn("Added new skin element to UI");

        
    }

    private static bool done = false;
}

[HarmonyPatch(typeof(SkinSelectorLogic), nameof(SkinSelectorLogic.SetCurrentElement))]
class y
{
    public static void Postfix(SkinSelectorLogic __instance)
    {
        ModLog.Error("Set selected");

        ModLog.Info(__instance.currentSkin);

        if (__instance.currentSkinIndex < 8)
            return;

        __instance.skinName.normalText.richText = true;
        __instance.skinName.shadowText.richText = true;
        __instance.skinName.SetText("Test Skin <color=#FFDE92>by</color> Damocles");
    }
}

static class test
{
    // Recursive method that returns the entire hierarchy of an object
    public static string DisplayHierarchy(this Transform transform, int maxLevel, bool includeComponents)
    {
        return transform.DisplayHierarchy_INTERNAL(new StringBuilder(), 0, maxLevel, includeComponents).ToString();
    }

    private static StringBuilder DisplayHierarchy_INTERNAL(this Transform transform, StringBuilder currentHierarchy, int currentLevel, int maxLevel, bool includeComponents)
    {
        // Indent
        for (int i = 0; i < currentLevel; i++)
            currentHierarchy.Append('\t');

        // Add this object
        currentHierarchy.Append(transform.name);

        // Add components
        if (includeComponents)
        {
            currentHierarchy.Append(" - ");
            foreach (Component c in transform.GetComponents<Component>())
                currentHierarchy.Append(c.GetIl2CppType().FullName + ", ");
        }
        currentHierarchy.AppendLine();

        // Add children
        if (currentLevel < maxLevel)
        {
            for (int i = 0; i < transform.childCount; i++)
                currentHierarchy = transform.GetChild(i).DisplayHierarchy_INTERNAL(currentHierarchy, currentLevel + 1, maxLevel, includeComponents);
        }

        // Return output
        return currentHierarchy;
    }
}
