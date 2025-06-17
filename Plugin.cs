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
        public static ConfigEntry<float>? ProneAdsDrainPercent;
        public static ConfigEntry<float>? CrouchAdsDrainPercent;

        private void Awake()
        {
            LogSource = Logger;
            LogSource.LogInfo("BetterArmStamina Plugin loading...");
            
            ModEnabled = Config.Bind(
                "General",
                "Enabled",
                true,
                "Should work or should not work."
            );

            ProneAdsDrainPercent = Config.Bind(
                "ADS Stamina Adjustment",
                "ProneAdsDrainPercent",
                30f,
                "ADS arm stamina drain rate as a percentage of normal when prone. (e.g., 30 for 30%, 100 for 100%)."
            );

            CrouchAdsDrainPercent = Config.Bind(
                "ADS Stamina Adjustment",
                "CrouchAdsDrainPercent",
                60f,
                "ADS arm stamina drain rate as a percentage of normal when crouching. (e.g., 60 for 60%, 100 for 100%)."
            );


            if (ModEnabled != null) ModEnabled.SettingChanged += OnConfigChanged;
            if (ProneAdsDrainPercent != null) ProneAdsDrainPercent.SettingChanged += OnConfigChanged;
            if (CrouchAdsDrainPercent != null) CrouchAdsDrainPercent.SettingChanged += OnConfigChanged;

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
            LogSource.LogInfo($"Core Mod: Enabled={ModEnabled?.Value ?? false} | ProneADS={ProneAdsDrainPercent?.Value ?? 0}% | CrouchADS={CrouchAdsDrainPercent?.Value ?? 0}%");
            LogSource.LogInfo("===============================================");
        }
    }
}