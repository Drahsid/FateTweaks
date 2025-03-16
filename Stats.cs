using HarmonyLib;
using System;
using Tableflip;
using UnityEngine;

namespace FateTweaks;

static class Stats
{
    [HarmonyPatch(typeof(Character))]
    [HarmonyPatch("AwardExperience")]
    [HarmonyPrefix]
    static bool Prefix__Character__AwardExperience(Character __instance, ref int award, ref bool fromMaster, ref bool isLoadSave)
    {
        if (PluginConfig.ExperienceMultiplier.Value != 1.0f)
        {
            award = (int)MathF.Round((float)award * PluginConfig.ExperienceMultiplier.Value);
        }

        return true;
    }

    [HarmonyPatch(typeof(Character))]
    [HarmonyPatch("AwardFame")]
    [HarmonyPrefix]
    static bool Prefix__Character__AwardFame(Character __instance, ref uint fame)
    {
        if (PluginConfig.FameMultiplier.Value != 1.0f)
        {
            fame = (uint)MathF.Round((float)fame * PluginConfig.FameMultiplier.Value);
        }

        return true;
    }

    // bad for shops
    /*[HarmonyPatch(typeof(PlayerStatus))]
    [HarmonyPatch("AddGold")]
    [HarmonyPrefix]
    static bool PrefixAddGold(PlayerStatus __instance, ref uint amount)
    {
        if (PluginConfig.GoldMultiplier.Value != 1.0f)
        {
            amount = (uint)MathF.Round((float)amount * PluginConfig.GoldMultiplier.Value);
        }

        return true;
    }*/

    [HarmonyPatch(typeof(GoldPileTrigger))]
    [HarmonyPatch("Init")]
    [HarmonyPrefix]
    static bool Prefix__GoldPileTrigger__Init(GoldPileTrigger __instance, ref int totalGold)
    {
        if (PluginConfig.GoldMultiplier.Value != 1.0f)
        {
            totalGold = (int)MathF.Round((float)totalGold * PluginConfig.GoldMultiplier.Value);
        }

        return true;
    }
}
