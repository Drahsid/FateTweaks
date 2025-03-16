using BepInEx.Configuration;

namespace FateTweaks;

static class PluginConfig
{
    public static ConfigEntry<bool> FishingRestoreLogicEnabled;
    public static ConfigEntry<float> FishingReactionTime;
    public static ConfigEntry<float> FishingSuccessMinChance;
    public static ConfigEntry<float> FishingSuccessMaxChance;

    public static ConfigEntry<bool> ItemSizeOverrideEnabled;
    public static ConfigEntry<int> ItemSizeOverrideTall;
    public static ConfigEntry<int> ItemSizeOverrideWide;

    public static ConfigEntry<float> ExperienceMultiplier;
    public static ConfigEntry<float> GoldMultiplier;
    public static ConfigEntry<float> FameMultiplier;

    private static ConfigFile _config;

    public static void Initialize(ConfigFile config)
    {
        _config = config;
        FishingRestoreLogicEnabled = config.Bind("Fishing",
            "Enabled",
            true,
            "When enabled, restores the original rng logic of fishing, as well as accounting for different fixed timestep.");

        FishingReactionTime = config.Bind("Fishing",
            "FishingReactionTime",
            0.15f,
            "Additional time wherein your chance to catch a fish is the highest, to account for reaction time. The behavior of the original game and reawakened would have this at 0.");

        FishingSuccessMinChance = config.Bind("Fishing",
            "FishingSuccessMinChance",
            0.25f,
            "Minimum value of the dice roll which determines your chance to succeed in reeling in a fish. If you want to always succeed, set this to 0 and the max value to 0. Default is 0.25");

        FishingSuccessMaxChance = config.Bind("Fishing",
            "FishingSuccessMaxChance",
            1.0f,
            "Minimum value of the dice roll which determines your chance to succeed in reeling in a fish. If you want to always succeed, set this to 0 and the max value to 0. Default is 1.0");

        ItemSizeOverrideEnabled = config.Bind("ItemSize",
            "Enabled",
            false,
            "When enabled, all items are changes to be as tall and wide as defined in the following values.");

        ItemSizeOverrideTall = config.Bind("ItemSize",
            "Tall",
            1,
            "Height of items within inventories when ItemSize override is enabled.");

        ItemSizeOverrideWide = config.Bind("ItemSize",
            "Wide",
            1,
            "Width of items within inventories when ItemSize override is enabled.");

        ExperienceMultiplier = config.Bind("Stats",
            "ExperienceMultiplier",
            1.0f,
            "The experience received is multiplied by this value.");

        GoldMultiplier = config.Bind("Stats",
            "GoldMultiplier",
            1.0f,
            "The gold received from gold piles is multiplied by this value.");

        FameMultiplier = config.Bind("Stats",
            "FameMultiplier",
            1.0f,
            "The fame received is multiplied by this value.");
    }
}
