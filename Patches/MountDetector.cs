using EFT;
using Comfort.Common;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace BetterArmStamina.Patches
{
    public static class MountDetector
    {
        private static bool _isMounted = false;
        private static bool _vKeyWasPressedLastFrame = false;
        private static readonly KeyCode MOUNT_KEY = KeyCode.V;
        private static int _mountKeyPressCount = 0;

        // Persistence to avoid flickering
        private static float _lastValidMountTime = 0f;
        private static readonly float MOUNT_PERSISTENCE_DURATION = 3.0f;
        private static bool _wasValidMountDetected = false;

        // Follow-up scanning
        private static float _nextScanTime = 0f;
        private static int _scanAttemptsSinceKeyPress = 0;
        private static readonly int MAX_SCAN_ATTEMPTS = 2;

        private static readonly KeyCode[] MOUNT_CANCEL_KEYS = new KeyCode[]
        {
            KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Q, KeyCode.E,
            KeyCode.Space, KeyCode.LeftShift, KeyCode.C, KeyCode.X, KeyCode.Z,
            KeyCode.R, KeyCode.Tab, KeyCode.Escape,
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5
        };

        public static bool IsPlayerMounted()
        {
            bool isVKeyPressed = Input.GetKey(MOUNT_KEY);
            bool keyJustPressed = isVKeyPressed && !_vKeyWasPressedLastFrame;
            float currentTime = Time.time;

            // Check for mount-canceling key presses
            if (_isMounted || _wasValidMountDetected)
            {
                foreach (KeyCode key in MOUNT_CANCEL_KEYS)
                {
                    if (Input.GetKeyDown(key))
                    {
                        bool wasInMountedState = _isMounted ||
                            (_wasValidMountDetected && (currentTime - _lastValidMountTime < MOUNT_PERSISTENCE_DURATION));

                        if (wasInMountedState)
                        {
                            Plugin.LogSource?.LogInfo($"[MountDetector] Mount cancelled by {key}");
                            _isMounted = false;
                            _wasValidMountDetected = false;
                        }
                        break;
                    }
                }
            }

            // Handle V key press
            if (keyJustPressed)
            {
                _mountKeyPressCount++;
                Plugin.LogSource?.LogInfo($"[MountDetector] V key pressed #{_mountKeyPressCount}");

                // If already mounted, dismount
                if (_isMounted || (_wasValidMountDetected && (currentTime - _lastValidMountTime < MOUNT_PERSISTENCE_DURATION)))
                {
                    Plugin.LogSource?.LogInfo("[MountDetector] Dismounting");
                    _isMounted = false;
                    _wasValidMountDetected = false;
                }
                else
                {
                    // Start scanning for mount indicators
                    _scanAttemptsSinceKeyPress = 0;
                    ScanForMountIndicators();
                    _nextScanTime = currentTime + 0.2f;
                }
            }

            // Perform follow-up scans
            if (_scanAttemptsSinceKeyPress < MAX_SCAN_ATTEMPTS && currentTime >= _nextScanTime)
            {
                _scanAttemptsSinceKeyPress++;
                ScanForMountIndicators();
                _nextScanTime = currentTime + 0.3f;
            }

            _vKeyWasPressedLastFrame = isVKeyPressed;

            // Return mount state (including persistence)
            bool shouldPersistMount = _wasValidMountDetected &&
                (currentTime - _lastValidMountTime < MOUNT_PERSISTENCE_DURATION);
            return _isMounted || shouldPersistMount;
        }

        private static void ScanForMountIndicators()
        {
            try
            {
                Image[] allImages = GameObject.FindObjectsOfType<Image>(false);
                bool foundValidIndicator = false;
                List<string> foundIndicators = new List<string>();

                foreach (var image in allImages)
                {
                    if (image == null || image.color.a <= 0.01f) continue;

                    string imageName = image.gameObject.name?.ToLowerInvariant() ?? "";
                    string spriteName = image.sprite?.name?.ToLowerInvariant() ?? "";

                    // Look for mount-related elements
                    bool isMountRelated = imageName.Contains("mount") || spriteName.Contains("mount") ||
                                         imageName.Contains("bipod") || spriteName.Contains("bipod");

                    if (isMountRelated)
                    {
                        // Skip unavailable/disabled indicators
                        if (imageName.Contains("unavailable") || spriteName.Contains("red") ||
                            imageName.Contains("disabled"))
                        {
                            continue;
                        }

                        // Valid mount indicator found
                        foundValidIndicator = true;
                        foundIndicators.Add($"{imageName}({spriteName})");
                    }
                }

                // Update mount state
                bool previousState = _isMounted;
                _isMounted = foundValidIndicator;

                if (foundValidIndicator)
                {
                    _wasValidMountDetected = true;
                    _lastValidMountTime = Time.time;
                }

                // Log state changes
                if (previousState != _isMounted)
                {
                    if (_isMounted)
                    {
                        Plugin.LogSource?.LogInfo($"[MountDetector] Mounted: {string.Join(", ", foundIndicators)}");
                    }
                    else if (!(Time.time - _lastValidMountTime < MOUNT_PERSISTENCE_DURATION))
                    {
                        Plugin.LogSource?.LogInfo("[MountDetector] Unmounted");
                        _wasValidMountDetected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.LogSource?.LogError($"[MountDetector] Scan error: {ex.Message}");
            }
        }
    }
}