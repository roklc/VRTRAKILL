﻿using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

using Valve.VR;
namespace Plugin.VRTRAKILL.VRPlayer.VRCamera.Patches
{
    [HarmonyPatch] static class CameraConverter
    {
        // ty huskvr you pretty
        public static GameObject Container;
        public static Camera DesktopWorldCam, DesktopUICam;

        [HarmonyPrefix] [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start))] static void Containerize(NewMovement __instance)
        {
            Container = new GameObject("Main Camera Rig");
            Container.transform.parent = Vars.MainCamera.transform.parent;

            // making the world bigger by making the rig bigger and lowering it down to the ground
            // real player's (v1's) size does not get affected = profit!!
            Container.transform.localScale = new Vector3(2, 2, 2);
            Container.transform.localPosition = new Vector3(0, -4, 0);
            Container.transform.localRotation = Vars.MainCamera.transform.rotation;

            Container.AddComponent<VRCameraController>();

            Vars.MainCamera.transform.parent = Container.transform;

            // Desktop View
            if (Vars.Config.VRSettings.DV.EnableDV)
            {
                DesktopWorldCam = new GameObject("Desktop World Camera").AddComponent<Camera>();
                DesktopWorldCam.gameObject.AddComponent<DesktopCamera>();

                DesktopUICam = new GameObject("Desktop UI Camera").AddComponent<Camera>();
                DesktopUICam.gameObject.AddComponent<DesktopUICamera>();
            }
        }

        [HarmonyPrefix] [HarmonyPatch(typeof(CameraController), nameof(CameraController.Start))] static void ConvertCameras(CameraController __instance)
        {
            while (__instance.cam == null && __instance.hudCamera == null) {}

            __instance.cam.nearClipPlane = .01f;
            __instance.cam.stereoTargetEye = StereoTargetEyeMask.Both;
            __instance.cam.depth++;

            __instance.hudCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            __instance.hudCamera.depth++;

            XRSettings.gameViewRenderMode = GameViewRenderMode.RightEye;

            // for some particular reason destroying it is a bad idea.
            GameObject.Find("Virtual Camera").SetActive(false);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CameraController), nameof(CameraController.Update))]
        [HarmonyPatch(typeof(CameraFrustumTargeter), nameof(CameraFrustumTargeter.Update))]
        [HarmonyPatch(typeof(CameraFrustumTargeter), nameof(CameraFrustumTargeter.LateUpdate))]
        static bool DoNothing()
        {
            // do nothing
            return false;
        }
    }
}
