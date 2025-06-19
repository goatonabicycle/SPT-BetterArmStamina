using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using static EFT.Player;
using EFT.Animations;
using System;

namespace BetterArmStamina.Patches
{
    internal class BetterArmStaminaPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player), nameof(Player.LateUpdate));
        }

        private static bool _wasAiming = false;
        private static EPlayerPose _lastPose = EPlayerPose.Stand;
        private static bool _wasMounted = false;
        private static int _frameCounter = 0;

        [PatchPrefix]
        static void Prefix(ref Player __instance)
        {
            if (!__instance.IsYourPlayer) return;

            ProceduralWeaponAnimation pwa = __instance.ProceduralWeaponAnimation;
            if (pwa == null) return;

            bool isModEnabled = Plugin.ModEnabled?.Value ?? false;
            if (!isModEnabled)
            {
                UpdatePreviousStates(pwa.IsAiming, __instance.Pose, MountDetector.IsPlayerMounted());
                return;
            }

            bool isAiming = pwa.IsAiming;
            EPlayerPose currentPose = __instance.Pose;
            bool isMounted = MountDetector.IsPlayerMounted();
            _frameCounter++;

            // Debug logging every 2 seconds
            if (_frameCounter % 120 == 0)
            {
                float currentMultiplier = __instance.Physical.HandsStamina.Multiplier;
                Plugin.LogSource?.LogInfo($"[ArmStamina] Multiplier: {currentMultiplier:F3} | Pose: {currentPose} | Aiming: {isAiming} | Mounted: {isMounted}");
            }

            // Handle aiming state changes
            if (isAiming && !_wasAiming)
            {
                // Started aiming
                ApplyMultiplier(__instance, isMounted, currentPose);
                Plugin.LogSource?.LogInfo($"[ArmStamina] Started aiming - {(isMounted ? "Mounted" : currentPose.ToString())}");
            }
            else if (isAiming && (currentPose != _lastPose || isMounted != _wasMounted))
            {
                // State changed while aiming
                ApplyMultiplier(__instance, isMounted, currentPose);
                Plugin.LogSource?.LogInfo($"[ArmStamina] State changed - {(isMounted ? "Mounted" : currentPose.ToString())}");
            }
            else if (!isAiming && _wasAiming)
            {
                // Stopped aiming
                __instance.Physical.HandsStamina.Multiplier = 1.0f;
                Plugin.LogSource?.LogInfo("[ArmStamina] Stopped aiming");
            }
            else if (isAiming && _frameCounter % 60 == 0)
            {
                // Periodic correction to handle edge cases
                float expected = GetMultiplier(isMounted, currentPose);
                float current = __instance.Physical.HandsStamina.Multiplier;

                if (Math.Abs(current - expected) > 0.01f)
                {
                    Plugin.LogSource?.LogInfo($"[ArmStamina] Correcting multiplier: {current:F3} -> {expected:F3}");
                    __instance.Physical.HandsStamina.Multiplier = expected;
                }
            }

            UpdatePreviousStates(isAiming, currentPose, isMounted);
        }

        private static void ApplyMultiplier(Player player, bool isMounted, EPlayerPose pose)
        {
            float multiplier = GetMultiplier(isMounted, pose);
            player.Physical.HandsStamina.Multiplier = multiplier;

            string state = isMounted ? "Mounted" : pose.ToString();
            Plugin.LogSource?.LogInfo($"[ArmStamina] Applied {multiplier:F3} ({state})");
        }

        private static float GetMultiplier(bool isMounted, EPlayerPose pose)
        {
            if (isMounted)
                return (Plugin.MountedOrBipodAdsDrainPercent?.Value ?? 30f) / 100f;

            return pose switch
            {
                EPlayerPose.Prone => (Plugin.ProneAdsDrainPercent?.Value ?? 30f) / 100f,
                EPlayerPose.Duck => (Plugin.CrouchAdsDrainPercent?.Value ?? 60f) / 100f,
                _ => (Plugin.StandingStaminaDrain?.Value ?? 100f) / 100f
            };
        }

        private static void UpdatePreviousStates(bool isAiming, EPlayerPose pose, bool isMounted)
        {
            _wasAiming = isAiming;
            _lastPose = pose;
            _wasMounted = isMounted;
        }
    }
}