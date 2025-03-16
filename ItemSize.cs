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
        __instance.slotsTall = 1;
        __instance.slotsWide = 1;
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetSprite")]
    static void PostfixItemSetSprite(Item __instance)
    {
        __instance.slotsTall = 1;
        __instance.slotsWide = 1;
    }
}
