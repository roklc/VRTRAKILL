﻿using UnityEngine;
using Valve.VR;

namespace Plugin.VRTRAKILL.VRPlayer.Controllers
{
    // lol the name
    public class ControllerController : MonoBehaviour
    {
        private SteamVR_RenderModel SVRRM; private SteamVR_Behaviour_Pose Pose;
        public GameObject GunOffset = new GameObject("Gun Offset") { layer = (int)Vars.Layers.IgnoreRaycast };
        public GameObject ArmOffset = new GameObject("Arm Offset") { layer = (int)Vars.Layers.IgnoreRaycast };

        GameObject Pointer;
        LineRenderer LR; Vector3 EndPosition;
        public float DefaultLength => Vars.Config.CBS.CrosshairDistance;

        private void SetupOffsets()
        {
            GunOffset.transform.parent = transform;
            GunOffset.transform.localPosition = Vector3.zero;
            GunOffset.transform.localRotation = Quaternion.Euler(45, 0, 0);

            ArmOffset.transform.parent = transform;
        }

        private void SetupControllerPointer()
        {
            // Create a real pointer with the camera for ui interaction
            Pointer = new GameObject("Canvas Pointer") { layer = (int)Vars.Layers.UI };
            Pointer.transform.parent = GunOffset.transform;

            Camera PointerCamera = Pointer.AddComponent<Camera>();
            PointerCamera.stereoTargetEye = StereoTargetEyeMask.None;
            PointerCamera.clearFlags = CameraClearFlags.Nothing;
            PointerCamera.cullingMask = -1; // Nothing
            PointerCamera.nearClipPlane = .01f;
            PointerCamera.fieldOfView = 1; // haha, ha, 1!
            PointerCamera.enabled = false;

            Pointer.AddComponent<UI.UIInteraction>();
        }
        private void SetupControllerLines()
        {
            LR = Pointer.AddComponent<LineRenderer>();
            LR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            LR.receiveShadows = false;
            LR.allowOcclusionWhenDynamic = false;
            LR.useWorldSpace = true;
            LR.material = new Material(Shader.Find("GUI/Text Shader"));

            Color C1 = new Color(1, 1, 1, Vars.Config.UIInteraction.ControllerLines.StartAlpha),
                  C2 = new Color(1, 1, 1, Vars.Config.UIInteraction.ControllerLines.EndAlpha);

            LR.startWidth = 0.02f; LR.endWidth = 0.001f;
            LR.startColor = C1; LR.endColor = C2;
        }

        private void CPRaycast()
        {
            bool Raycast = Physics.Raycast(GunOffset.transform.position, GunOffset.transform.forward,
                                           out RaycastHit Hit, float.PositiveInfinity, (int)Vars.Layers.UI);
            EndPosition = GunOffset.transform.position + (GunOffset.transform.forward * DefaultLength);
            if (Raycast) EndPosition = Hit.point;
        }
        private void DrawControllerLines()
        {
            if (Vars.IsPlayerFrozen || Vars.IsPlayerUsingShop) LR.enabled = true;
            else LR.enabled = false;

            if (LR.enabled)
            {
                LR.SetPosition(0, GunOffset.transform.position);
                LR.SetPosition(1, EndPosition);
            }
        }

        public void Start()
        {
            SVRRM = GetComponentInChildren<SteamVR_RenderModel>();
            Pose = GetComponent<SteamVR_Behaviour_Pose>();

            SetupOffsets();

            if (Vars.Config.UIInteraction.ControllerBased) SetupControllerPointer();
            if (Vars.Config.UIInteraction.ControllerLines.Enabled) SetupControllerLines();
        }
        public void Update()
        {
            // controller model
            if (Vars.Config.Controllers.DrawControllers && Vars.IsMainMenu)
                try { SVRRM.gameObject.SetActive(true); } catch {}
            else try { SVRRM.gameObject.SetActive(false); } catch {}

            // paused
            if (Vars.IsPaused && !Vars.IsMainMenu) Pose.enabled = false;
            else Pose.enabled = true;

            // controller-based ui interaction
            if (Vars.Config.UIInteraction.ControllerBased) CPRaycast();
            if (Vars.Config.UIInteraction.ControllerLines.Enabled) DrawControllerLines();
        }

        public static Vector3 ControllerOffset = new Vector3(0, 2.85f, 0);
        public static void onTransformUpdatedH(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource)
        => fromAction.transform.position += ControllerOffset;
    }
}