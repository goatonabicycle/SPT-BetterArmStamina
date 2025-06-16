using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using MoreRealisticArmStamina.Patches;

namespace MoreRealisticArmStamina
{
    [BepInPlugin("goatonabicycle.MoreRealisticArmStamina", "MoreRealisticArmStamina", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;
        public static ConfigEntry<bool> ModEnabled;
        public static ConfigEntry<float> ProneAdsDrainPercent;
        public static ConfigEntry<float> CrouchAdsDrainPercent;

        private void Awake()
        {
            LogSource = Logger;
            LogSource.LogInfo("MoreRealisticArmStamina Plugin loaded!");

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
                "ADS arm stamina drain rate as a percentage of normal when prone. (e.g., 30 for 30%, 100 for 100%)"
            );

            CrouchAdsDrainPercent = Config.Bind(
                "ADS Stamina Adjustment",
                "CrouchAdsDrainPercent",
                60f,
                "ADS arm stamina drain rate as a percentage of normal when crouching. (e.g., 60 for 60%, 100 for 100%)"
            );

            new ArmStaminaPatch().Enable();
        }
    }
}