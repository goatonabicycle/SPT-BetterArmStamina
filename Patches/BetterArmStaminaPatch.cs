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
        
        private static int _debugLogCounter = 0;
        private static int _debugLogFrequency = 60;

        [PatchPrefix]
        static void Prefix(ref Player __instance)
        {            
            if (!__instance.IsYourPlayer)
                return;
            
            ProceduralWeaponAnimation pwa = __instance.ProceduralWeaponAnimation;
            if (pwa == null)
                return;
                
            bool isAiming = pwa.IsAiming;
            EPlayerPose currentPose = __instance.Pose;
            float currentMultiplier = __instance.Physical.HandsStamina.Multiplier;
            bool isModEnabled = Plugin.ModEnabled?.Value ?? false;
            
            bool shouldLog = (_debugLogCounter++ % _debugLogFrequency == 0);
            if (shouldLog)
            {
                Plugin.LogSource?.LogInfo($"[ArmStamina] Multiplier: {currentMultiplier:F3} | Mod: {(isModEnabled ? "ON" : "OFF")} | Pose: {currentPose} | Aiming: {isAiming}");
            }
            
            if (!isModEnabled)
            {
                _wasAiming = isAiming;
                return;
            }
            
            if (isAiming && !_wasAiming)
            {                
                float drainPercent = currentPose switch
                {
                    EPlayerPose.Prone => Plugin.ProneAdsDrainPercent?.Value ?? 30f,
                    EPlayerPose.Duck => Plugin.CrouchAdsDrainPercent?.Value ?? 60f,
                    _ => 100f
                };
                             
                float newMultiplier = drainPercent / 100f;
                __instance.Physical.HandsStamina.Multiplier = newMultiplier;
                
                Plugin.LogSource?.LogInfo($"[ArmStamina] Started aiming - Applied multiplier: {newMultiplier:F3} (Stance: {currentPose})");
            }            
            else if (isAiming && currentPose != _lastPose)
            {             
                float drainPercent = currentPose switch
                {
                    EPlayerPose.Prone => Plugin.ProneAdsDrainPercent?.Value ?? 30f,
                    EPlayerPose.Duck => Plugin.CrouchAdsDrainPercent?.Value ?? 60f,
                    _ => 100f
                };
                
                float newMultiplier = drainPercent / 100f;
                __instance.Physical.HandsStamina.Multiplier = newMultiplier;
                
                Plugin.LogSource?.LogInfo($"[ArmStamina] Stance changed - Applied multiplier: {newMultiplier:F3} (Stance: {currentPose})");
            }            
            else if (!isAiming && _wasAiming)
            {                
                __instance.Physical.HandsStamina.Multiplier = 1.0f;
                
                Plugin.LogSource?.LogInfo($"[ArmStamina] Stopped aiming - Reset multiplier to 1.0");
            }
                        
            _wasAiming = isAiming;
            _lastPose = currentPose;
        }
    }
}