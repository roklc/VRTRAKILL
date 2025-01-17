﻿using UnityEngine;

namespace Plugin.VRTRAKILL.VRPlayer.Controllers
{
    public class GunController : MonoSingleton<GunController>
    {
        public ControllerController CC;
        public GameObject GunOffset;

        public Vector3 ArmOffset = new Vector3(.05f, .0525f, -.1765f);

        public void Start()
        {
            CC = gameObject.GetComponent<ControllerController>();
            GunOffset = CC.GunOffset;
        }

        public void Update() => CC.ArmOffset.transform.localPosition = ArmOffset;
    }
}
