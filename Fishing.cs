using HarmonyLib;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using Tableflip;
using Cleverous.ThatHurtModule;

namespace FateTweaks;

[HarmonyPatch(typeof(FishingSpotInteractable))]
static class Fishing
{
    const float FISHING_BASE_BITE_CHANCE = 16.0f;
    const float FISHING_ORIGINAL_TIMESTEP = 1.0f / 60.0f;
    static float biteSpeed = 1.0f;

    [HarmonyPrefix]
    [HarmonyPatch("FishingUpdate")]
    static bool Prefix__FishingSpotInteractable__FishingUpdate(ref IEnumerator __result, FishingSpotInteractable __instance, Character owner)
    {
        Plugin.Logger.LogInfo("FishingUpdate is being called!");
        if (PluginConfig.FishingRestoreLogicEnabled.Value)
        {
            __result = FishingUpdate(__instance, owner);
            return false;
        }

        return true;
    }

    static IEnumerator FishingUpdate(FishingSpotInteractable fishingSpotInteractable, Character owner)
    {
        for (;;)
        {
            if (fishingSpotInteractable.isFirstCast)
            {
                yield return new WaitForSeconds(2.0f);
                SimpleSingletonBehaviour<PlayerUI>.Instance.setHookButton.onClick.AddListener(new UnityAction(fishingSpotInteractable.SetHook));
                fishingSpotInteractable.isFirstCast = false;
            }
            fishingSpotInteractable.caugthFish = false;
            if (!fishingSpotInteractable.pullFish)
            {
                int scaledChance = Mathf.RoundToInt(FISHING_BASE_BITE_CHANCE * (Time.fixedDeltaTime / FISHING_ORIGINAL_TIMESTEP));
                int rng = UnityEngine.Random.Range(0, 20000);
                Plugin.Logger.LogDebug($"[Fishing Bite] rng: {rng}, chance: {scaledChance}, rng < chance: {rng < scaledChance}");
                if (rng < scaledChance)
                {
                    fishingSpotInteractable.timeSinceBite = 0.0f;
                    biteSpeed = UnityEngine.Random.Range(0.2f, 1.0f);
                    fishingSpotInteractable.biteTime = 1.0f / biteSpeed;
                    fishingSpotInteractable.fishBite = true;
                    Player.PlayerChar.SetFishBiting(true);
                    Plugin.Logger.LogDebug($"[Fishing Bite] got a bite! biteSpeed: {biteSpeed}, biteTime: {fishingSpotInteractable.biteTime}");
                }
                if (fishingSpotInteractable.fishBite)
                {
                    fishingSpotInteractable.timeSinceBite += Time.fixedDeltaTime * biteSpeed;
                    if (1.0f - fishingSpotInteractable.timeSinceBite <= 0.0f)
                    {
                        Plugin.Logger.LogDebug("[Fishing Bite] Missed bite!");
                        fishingSpotInteractable.fishBite = false;
                        Player.PlayerChar.SetFishBiting(false);
                    }
                }
            }
            if (fishingSpotInteractable.pullFish && fishingSpotInteractable.fishBite)
            {
                float rng = UnityEngine.Random.Range(PluginConfig.FishingSuccessMinChance.Value, PluginConfig.FishingSuccessMaxChance.Value);
                float reactionTime = PluginConfig.FishingReactionTime.Value * biteSpeed;
                float chance = (1.0f + reactionTime) - fishingSpotInteractable.timeSinceBite;
                chance = Mathf.Clamp01(chance);

                Plugin.Logger.LogDebug($"[Fishing Reel] chance: {chance}, rng: {rng}, min {PluginConfig.FishingSuccessMinChance.Value}, max {PluginConfig.FishingSuccessMaxChance.Value}");
                if (rng <= chance)
                {
                    fishingSpotInteractable.caugthFish = true;
                    Player.PlayerChar.IsFishCaught(true);
                }
                else
                {
                    ThatHurt.PopText(fishingSpotInteractable.transform.position, I2Loca.Get("Failed"), 1.0f, ThatHurt.ThatHurtColor.White, 0.0f);
                    Player.PlayerChar.IsFishCaught(false);
                }
                if (fishingSpotInteractable.caugthFish)
                {
                    fishingSpotInteractable.GiveAward(owner);
                    fishingSpotInteractable.caugthFish = false;
                    fishingSpotInteractable.fishBite = false;
                    StatsManager.IncrementStat(Stat.FishCaught, 1, false);
                }
                yield return new WaitForSeconds(4.0f);
                fishingSpotInteractable.pullFish = false;
            }
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
}
