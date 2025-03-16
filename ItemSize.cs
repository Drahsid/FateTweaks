using HarmonyLib;
using System;

using Tableflip;

namespace FateTweaks;

[HarmonyPatch(typeof(Item))]
static class ItemSize
{
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPostfix]
    static void PostfixItemConstructor(Item __instance)
    {
        if (PluginConfig.ItemSizeOverrideEnabled.Value)
        {
            __instance.slotsTall = PluginConfig.ItemSizeOverrideTall.Value;
            __instance.slotsWide = PluginConfig.ItemSizeOverrideWide.Value;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetSprite")]
    static void PostfixItemSetSprite(Item __instance)
    {
        if (PluginConfig.ItemSizeOverrideEnabled.Value)
        {
            __instance.slotsTall = PluginConfig.ItemSizeOverrideTall.Value;
            __instance.slotsWide = PluginConfig.ItemSizeOverrideWide.Value;
        }
    }
}
