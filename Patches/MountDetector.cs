using Comfort.Common;
using EFT;
using System.Reflection;
using HarmonyLib;
using System;

namespace BetterArmStamina.Patches
{
    public static class MountDetector
    {
        private static FieldInfo _inMountedStateField;

        public static bool IsPlayerMounted()
        {
            var player = Singleton<GameWorld>.Instance?.MainPlayer;
            if (player == null)
                return false;

            var movementContext = player.MovementContext;

            if (_inMountedStateField == null)
            {
                try
                {
                    _inMountedStateField = AccessTools.Field(typeof(MovementContext), "_inMountedState");
                    if (_inMountedStateField == null)
                    {
                        Plugin.LogSource?.LogError("[MountDetector] Failed to find _inMountedState field via reflection.");
                    }
                }
                catch (Exception ex)
                {
                    Plugin.LogSource?.LogError($"[MountDetector] Exception during _inMountedState reflection lookup: {ex.Message}");
                }
            }

            if (_inMountedStateField != null)
            {
                try
                {
                    return (bool)_inMountedStateField.GetValue(movementContext);
                }
                catch (Exception ex)
                {
                    Plugin.LogSource?.LogError($"[MountDetector] Failed to get _inMountedState value via reflection: {ex.Message}");
                    _inMountedStateField = null;
                }
            }

            return movementContext.CurrentState.Name == EPlayerState.Stationary;
        }
    }
}