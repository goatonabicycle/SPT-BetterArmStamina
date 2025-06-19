using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BetterArmStamina.Patches;
using System;

namespace BetterArmStamina
{
    [BepInPlugin("goatonabicycle.BetterArmStamina", "BetterArmStamina", "1.0.0")]
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
            LogSource.LogInfo("BetterArmStamina Plugin loading..."); ModEnabled = Config.Bind(
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

            if (ModEnabled != null) ModEnabled.SettingChanged += OnConfigChanged;
            if (StandingAdsStaminaDrain != null) StandingAdsStaminaDrain.SettingChanged += OnConfigChanged;
            if (CrouchingAdsStaminaDrain != null) CrouchingAdsStaminaDrain.SettingChanged += OnConfigChanged;
            if (ProneAdsStaminaDrain != null) ProneAdsStaminaDrain.SettingChanged += OnConfigChanged;
            if (MountedAdsStaminaDrain != null) MountedAdsStaminaDrain.SettingChanged += OnConfigChanged;

            try
            {
                new BetterArmStaminaPatch().Enable();
                LogSource.LogInfo("BetterArmStamina patch applied successfully!");
            }
            catch (Exception ex)
            {
                LogSource?.LogError($"Failed to apply patches: {ex.Message}");
                LogSource?.LogError(ex.StackTrace);
            }

            LogConfiguration();
        }

        private void OnConfigChanged(object sender, EventArgs e)
        {
            LogConfiguration();
        }

        private void LogConfiguration()
        {
            if (LogSource == null) return;
            LogSource.LogInfo("========== BETTER ARM STAMINA CONFIG ==========");
            LogSource.LogInfo($"Enabled: {ModEnabled?.Value ?? false} | Standing: {StandingAdsStaminaDrain?.Value ?? 100}% | Crouching: {CrouchingAdsStaminaDrain?.Value ?? 60}% | Prone: {ProneAdsStaminaDrain?.Value ?? 30}% | Mounted: {MountedAdsStaminaDrain?.Value ?? 30}%");
            LogSource.LogInfo("===============================================");
        }
    }
}