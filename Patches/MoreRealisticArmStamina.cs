using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using static EFT.Player;
using EFT.Animations;
using System;

namespace MoreRealisticArmStamina.Patches
{
    internal class ArmStaminaPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player), nameof(Player.LateUpdate));
        }

        private static bool _wasAimingPreviously = false;
        private static float _originalMultiplier;
        private static bool _hasStoredOriginalMultiplier = false;

        [PatchPrefix]
        static void Prefix(ref Player __instance)
        {
            if (!Plugin.ModEnabled.Value || !__instance.IsYourPlayer)
                return;

            ProceduralWeaponAnimation pwa = __instance.ProceduralWeaponAnimation;
            if (pwa == null)
                return;

            bool isPlayerAiming = pwa.IsAiming;
            float currentMultiplier = __instance.Physical.HandsStamina.Multiplier;

            if (!_hasStoredOriginalMultiplier)
            {
                _originalMultiplier = currentMultiplier;
                _hasStoredOriginalMultiplier = true;
            }

            if (isPlayerAiming)
            {
                float drainPercent = __instance.Pose switch
                {
                    EPlayerPose.Prone => Plugin.ProneAdsDrainPercent.Value,
                    EPlayerPose.Duck => Plugin.CrouchAdsDrainPercent.Value,
                    _ => 100f
                };

                float newMultiplier = drainPercent / 100f;
                if (Math.Abs(currentMultiplier - newMultiplier) > 0.001f)
                {
                    __instance.Physical.HandsStamina.Multiplier = newMultiplier;
                }
            }
            else if (_wasAimingPreviously)
            {
                __instance.Physical.HandsStamina.Multiplier = _originalMultiplier;
            }

            _wasAimingPreviously = isPlayerAiming;
        }
    }
}