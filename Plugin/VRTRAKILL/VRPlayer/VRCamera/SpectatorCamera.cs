﻿using System.Linq;
using UnityEngine;

namespace Plugin.VRTRAKILL.VRPlayer.VRCamera
{
    public enum SCMode
    {
        Follow = 0,
        RotateAround = 1,
        Fixed = 2
    }
    internal class SpectatorCamera : MonoSingleton<SpectatorCamera>
    {
        public SCMode Mode = SCMode.Follow;

        public Camera SPCam;

        public Vector3 OffsetPos = new Vector3(0, 1, 3);
        public float RAMRotationSpeed = .2f, FMDuration = 2;
        public Vector3 RotAngles = new Vector3(20, 0, 0);

        public override void OnEnable()
        {
            base.OnEnable();
            SPCam = GetComponentInChildren<Camera>();
            transform.eulerAngles = RotAngles;
        }

        public void Update()
        {
            transform.position = Vars.MainCamera.transform.position;
            SPCam.transform.localPosition = OffsetPos;
            Helpers.Misc.CopyCameraValues(SPCam, Vars.DesktopCamera);

            switch (Mode)
            {
                case SCMode.Follow: Follow(); break;
                case SCMode.RotateAround: RotateAround(); break;
                case SCMode.Fixed: transform.eulerAngles = RotAngles; break;
                default: break;
            }
        }

        private void Follow()
        { transform.forward = Vector3.Lerp(transform.forward, Vars.MainCamera.transform.forward, Time.deltaTime * FMDuration); }
        private void RotateAround()
        { transform.Rotate(new Vector3(0, RAMRotationSpeed, 0)); }
        
        public void EnumSCMode() // ugly
        {
            transform.eulerAngles = RotAngles;
            Mode++;
            if ((int)Mode > System.Enum.GetValues(typeof(SCMode)).Cast<int>().Max())
                Mode = 0;
        }
    }
}
