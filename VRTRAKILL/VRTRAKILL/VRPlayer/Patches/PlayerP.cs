﻿using HarmonyLib;
using UnityEngine;

namespace Plugin.VRTRAKILL.VRPlayer.Patches
{
    
    [HarmonyPatch(typeof(NewMovement))] internal class PlayerP
    {
        // before the patch the player was like TWICE as big and was TWICE as faster
        [HarmonyPrefix] [HarmonyPatch(nameof(NewMovement.Start))] static void SetupVRController(NewMovement __instance)
        {
            __instance.gameObject.AddComponent<Movement.VRPlayerController>();
        }
    }
}