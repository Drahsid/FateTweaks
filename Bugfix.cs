using HarmonyLib;
using System.Collections.Generic;
using Tableflip;

namespace FateTweaks;

static class Bugfix
{
    private static bool fixStats = false;

    private interface IEffectLike
    {
        EffectType BuffType { get; }
        float Value { get; }
        string Name { get; }
        float Duration { get; }
    }

    private class AppliedEffectAdapter : IEffectLike
    {
        private readonly AppliedEffect _effect;

        public AppliedEffectAdapter(AppliedEffect effect)
        {
            _effect = effect;
        }

        public EffectType BuffType => _effect.BuffType;
        public float Value => _effect.Value;
        public string Name => _effect.Name;
        public float Duration => _effect.duration;
    }

    private class EffectDataAdapter : IEffectLike
    {
        private readonly EffectData _effect;

        public EffectDataAdapter(EffectData effect)
        {
            _effect = effect;
        }

        public EffectType BuffType => _effect.buffType;
        public float Value => _effect.value;
        public string Name => _effect.name;
        public float Duration => _effect.duration;
    }

    private static float GetPercentCastSpeed<T>(Dictionary<ActivationType, List<T>> effects, System.Func<T, IEffectLike> adapter)
    {
        float totalValue = 0.0f;

        foreach (var pair in effects)
        {
            for (int i = 0; i < pair.Value.Count; i++)
            {
                IEffectLike effect = adapter(pair.Value[i]);

                if (effect.BuffType == EffectType.PercentCastSpeed)
                {
                    totalValue += effect.Value;
                    Plugin.Logger.LogDebug($"<{(typeof(T) == typeof(AppliedEffect) ? "Applied" : "Equipment")}> [{pair.Key}] PercentCastSpeed from {effect.Name} -> {effect.Value} for {effect.Duration}");
                }
            }
        }

        return totalValue;
    }

    private static float GetPercentCastSpeedFromDict(Dictionary<ActivationType, List<AppliedEffect>> effects)
    {
        return GetPercentCastSpeed(effects, effect => new AppliedEffectAdapter(effect));
    }

    private static float GetPercentCastSpeedFromDict(Dictionary<ActivationType, List<EffectData>> effects)
    {
        return GetPercentCastSpeed(effects, effect => new EffectDataAdapter(effect));
    }

    public static void RecalculatePercentCastSpeed(Character __instance)
    {
        float appliedEffectsValue = GetPercentCastSpeedFromDict(__instance.effects);
        float equipmentEffectsValue = 0.0f;

        foreach (var item in __instance.inventory)
        {
            if (item.equipped)
            {
                equipmentEffectsValue += GetPercentCastSpeedFromDict(item.effects);
            }
        }

        Plugin.Logger.LogDebug($"TOTAL: {appliedEffectsValue} / {equipmentEffectsValue}");
        if (appliedEffectsValue < equipmentEffectsValue)
        {
            fixStats = true;
        }
    }

    private static void RemovePercentCastSpeedEffects(Player player)
    {
        foreach (var pair in player.effects)
        {
            for (int i = 0; i < pair.Value.Count; i++)
            {
                var effect = pair.Value[i];
                if (effect.BuffType == EffectType.PercentCastSpeed && pair.Key == ActivationType.Passive)
                {
                    player.RemoveEffect(effect);
                }
            }
        }
    }

    private static void AddPercentCastSpeedEffectsFromEquipment(Player player)
    {
        foreach (var item in player.inventory)
        {
            if (item.equipped)
            {
                foreach (var pair in item.effects)
                {
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        var effect = pair.Value[i];
                        if (effect.buffType == EffectType.PercentCastSpeed && pair.Key == ActivationType.Passive)
                        {
                            player.AddEffect(effect, item.baseLevel, player, item.slotIndex);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    [HarmonyPostfix]
    static void Postfix__Player__Update(Player __instance)
    {
        if (fixStats)
        {
            RemovePercentCastSpeedEffects(__instance);
            AddPercentCastSpeedEffectsFromEquipment(__instance);
            fixStats = false;
            __instance.CalculateEffectValues();
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.CalculateEffectValue))]
    [HarmonyPostfix]
    static float Postfix__Player__CalculateEffectValue(float __result, Character __instance, ref EffectType type)
    {
        if (type == EffectType.PercentCastSpeed && fixStats == false && __instance.IsPlayer())
        {
            Plugin.Logger.LogWarning($"CalculateEffectValue: {type} is {__result}");
            RecalculatePercentCastSpeed(__instance);
        }
        return __result;
    }

    /*[HarmonyPatch(typeof(Player), nameof(Player.Init))]
    [HarmonyPatch(new[] { typeof(PlayerSaveData), typeof(ItemStorage) })]
    [HarmonyPostfix]
    static void Postfix__Player__Init(Player __instance)
    {
        RecalculatePercentCastSpeed(__instance);
    }*/
}
