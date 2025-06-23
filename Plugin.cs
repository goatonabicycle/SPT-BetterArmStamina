using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BetterArmStamina.Patches;
using System;

namespace BetterArmStamina
{
    [BepInPlugin("goatonabicycle.BetterArmStamina", "BetterArmStamina", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource? LogSource;
        public static ConfigEntry<bool>? ModEnabled;
        public static ConfigEntry<float>? StandingAdsStaminaDrain;
        public static ConfigEntry<float>? CrouchingAdsStaminaDrain;
        public static ConfigEntry<float>? ProneAdsStaminaDrain;
        public static ConfigEntry<float>? MountedAdsStaminaDrain;

        private void Awake()
        {
            LogSource = Logger;
            LogSource.LogInfo("BetterArmStamina Plugin loading...");

            ModEnabled = Config.Bind(
                "1. General",
                "Mod Enabled",
                true,
                "Enable or disable the entire mod."
            );

            StandingAdsStaminaDrain = Config.Bind(
                "2. ADS Stamina Settings",
                "Standing ADS Drain",
                100f,
                "Stamina drain when aiming down sights while standing (percentage, default 100%)."
            );

            CrouchingAdsStaminaDrain = Config.Bind(
                "2. ADS Stamina Settings",
                "Crouching ADS Drain",
                60f,
                "Stamina drain when aiming down sights while crouching (percentage, default 60%)."
            );

            ProneAdsStaminaDrain = Config.Bind(
                "2. ADS Stamina Settings",
                "Prone ADS Drain",
                30f,
                "Stamina drain when aiming down sights while prone (percentage, default 30%)."
            );

            MountedAdsStaminaDrain = Config.Bind(
                "2. ADS Stamina Settings",
                "Mounted ADS Drain",
                30f,
                "Stamina drain when aiming down sights while mounted or using a bipod (percentage, default 30%)."
            );

            try
            {
                new BetterArmStaminaPatch().Enable();
            }
            catch (Exception ex)
            {
                LogSource?.LogError($"Failed to apply patches: {ex.Message}");
                LogSource?.LogError(ex.StackTrace);
            }
        }
    }
}