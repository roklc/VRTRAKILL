﻿using UnityEngine;
using System.Linq;
using System.Collections;
using Plugin.VRTRAKILL.VRPlayer.VRCamera;

namespace Plugin.VRTRAKILL.VRPlayer
{
    // acts as many things in one
    internal class VRController : MonoSingleton<VRController>
    {
        private bool WasDVActive, IsSCKMode = false;

        public void Start()
        {
            if (Vars.Config.DesktopView.Enabled)
            {
                if (Vars.Config.DesktopView.SpectatorCamera.Enabled)
                    Vars.SpectatorCamera.SetActive(true);
                else Vars.DesktopCamera.gameObject.SetActive(true);
                Vars.DesktopUICamera.gameObject.SetActive(true);
            }

            if (SpectatorCamera.Instance != null)
                if (Vars.Config.DesktopView.SpectatorCamera.Mode <= 2 || Vars.Config.DesktopView.SpectatorCamera.Mode >= 0)
                    SpectatorCamera.Instance.Mode = (SCMode)Vars.Config.DesktopView.SpectatorCamera.Mode;
                else SpectatorCamera.Instance.Mode = 0;
        }

        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(Vars.Config.VRKeybinds.ToggleDV))
            {
                SubtitleController.Instance.DisplaySubtitle("VR: Toggling desktop view");
                ToggleDesktopView();
            }
            if (UnityEngine.Input.GetKeyDown(Vars.Config.VRKeybinds.ToggleSC))
            {
                SubtitleController.Instance.DisplaySubtitle("VR: Toggling spectator camera");
                ToggleSpectatorCamera();
            }
            if (UnityEngine.Input.GetKeyDown(Vars.Config.VRKeybinds.EnumSCMode))
            {
                SpectatorCamera.Instance.EnumSCMode();
                SubtitleController.Instance.DisplaySubtitle
                    ($"VR: Switched spectator camera mode to {System.Enum.GetName(typeof(SCMode), SpectatorCamera.Instance.Mode)}");
            }
        }

        private void ToggleDesktopView()
        {
            Vars.SpectatorCamera.SetActive(false);

            if (!Vars.DesktopCamera.gameObject.activeSelf) Vars.DesktopCamera.gameObject.SetActive(true);
            else Vars.DesktopCamera.gameObject.SetActive(false);

            if (!Vars.DesktopUICamera.gameObject.activeSelf) Vars.DesktopUICamera.gameObject.SetActive(true);
            else if (Vars.DesktopUICamera.gameObject.activeSelf && !Vars.SpectatorCamera.activeSelf)
                Vars.DesktopUICamera.gameObject.SetActive(false);
        }
        private void ToggleSpectatorCamera()
        {
            

            if (!Vars.SpectatorCamera.activeSelf)
            {
                if (Vars.DesktopCamera.gameObject.activeSelf) { WasDVActive = true; Vars.DesktopCamera.gameObject.SetActive(false); }
                Vars.SpectatorCamera.SetActive(true);

                if (!Vars.DesktopUICamera.gameObject.activeSelf)
                    Vars.DesktopUICamera.gameObject.SetActive(true);
            }
            else
            {
                if (WasDVActive) { Vars.DesktopCamera.gameObject.SetActive(true); WasDVActive = false; }
                else Vars.DesktopUICamera.gameObject.SetActive(false);
                Vars.SpectatorCamera.gameObject.SetActive(false);
            }
        }
    }
}
