﻿using Plugin.VRTRAKILL.VRPlayer.Controllers;
using UnityEngine;

namespace Plugin.VRTRAKILL.VRPlayer.VRIK
{
    internal class VRigController : MonoBehaviour
    {
        private static VRigController _Instance; public static VRigController Instance { get { return _Instance; } }

        public MetaRig Rig;

        public Vector3 LFBArmScale = new Vector3(0.0125f, 0.0125f, 0.0125f),
                       LArmScale = new Vector3(0.125f, 0.125f, 0.125f),
                       RArmScale = new Vector3(-0.0125f, 0.0125f, 0.0125f);

        public void Awake()
        {
            if (_Instance != null && _Instance != this) Destroy(this.gameObject);
            else _Instance = this;
        }

        public void Start()
        {
            if (Rig == null) Rig = MetaRig.CreateV1CustomPreset(Vars.VRCameraContainer);
            Helpers.Misc.RecursiveChangeLayer(Rig.GameObjectT.gameObject, (int)Vars.Layers.AlwaysOnTop);

            // enable all of the stuff because i forgot to do it in the editor
            Rig.LFeedbacker.GameObjecT.gameObject.SetActive(true);
            Rig.LKnuckleblaster.GameObjecT.gameObject.SetActive(true);
            Rig.LWhiplash.GameObjecT.gameObject.SetActive(true);
            Rig.RFeedbacker.GameObjecT.gameObject.SetActive(true);
            Rig.RSandboxer.GameObjecT.gameObject.SetActive(true);

            // transform shenanigans
            Rig.GameObjectT.localPosition = Vector3.zero;
            Rig.GameObjectT.localRotation = Quaternion.Euler(Vector3.zero);

            Rig.LForearm_Pole.localPosition = new Vector3(Rig.LForearm_Pole.localPosition.x, -.002f, Rig.LForearm_Pole.localPosition.z);
            Rig.RForearm_Pole.localPosition = new Vector3(Rig.RForearm_Pole.localPosition.x, -.002f, Rig.RForearm_Pole.localPosition.z);

            Rig.Root.localScale *= 3;
            Rig.Body.localPosition = new Vector3(0.0003f, -0.0145f, 0.003f);

            // for now don't use these GOs
            Rig.LeftLeg.GameObjecT.localScale = Vector3.zero;
            Rig.RightLeg.GameObjecT.localScale = Vector3.zero;
            Rig.Head.localScale = Vector3.zero;

            // left handed mode support
            if (Vars.Config.Controllers.HandS.LeftHandMode)
            {
                Vector3 TempLAPos = Rig.LFeedbacker.GameObjecT.localPosition,
                        TempRAPos = Rig.RFeedbacker.GameObjecT.localPosition;
                Rig.LFeedbacker.GameObjecT.localPosition = TempRAPos;
                Rig.RFeedbacker.GameObjecT.localPosition = TempLAPos;

                Rig.LFeedbacker.GameObjecT.localScale = new Vector3(Rig.LFeedbacker.GameObjecT.localScale.x * -1,
                                                                    Rig.LFeedbacker.GameObjecT.localScale.y,
                                                                    Rig.LFeedbacker.GameObjecT.localScale.z);
                Rig.RFeedbacker.GameObjecT.localScale = new Vector3(Rig.RFeedbacker.GameObjecT.localScale.x * -1,
                                                                    Rig.RFeedbacker.GameObjecT.localScale.y,
                                                                    Rig.RFeedbacker.GameObjecT.localScale.z);
            }

            // add vrik (pain and boilerplate)
            IKArm LFIK = Rig.LFeedbacker.Hand.Root.gameObject.AddComponent<IKArm>();
            LFIK.Target = ArmController.Instance.CC.ArmOffset.transform;
            LFIK.ChainLength = 3; LFIK.Pole = Rig.LForearm_Pole;

            IKArm LKIK = Rig.LKnuckleblaster.Hand.Root.gameObject.AddComponent<IKArm>();
            LKIK.Target = ArmController.Instance.CC.ArmOffset.transform;
            LKIK.ChainLength = 3; LKIK.Pole = Rig.LForearm_Pole;

            IKArm LWIK = Rig.LWhiplash.Hand.Root.gameObject.AddComponent<IKArm>();
            LWIK.Target = ArmController.Instance.CC.ArmOffset.transform;
            LWIK.ChainLength = 3; LWIK.Pole = Rig.LForearm_Pole;


            IKArm RFIK = Rig.RFeedbacker.Hand.Root.gameObject.AddComponent<IKArm>();
            RFIK.Target = GunController.Instance.CC.ArmOffset.transform;
            RFIK.ChainLength = 3; RFIK.Pole = Rig.RForearm_Pole;

            IKArm RSIK = Rig.RSandboxer.Hand.Root.gameObject.AddComponent<IKArm>();
            RSIK.Target = GunController.Instance.CC.ArmOffset.transform;
            RSIK.ChainLength = 3; RSIK.Pole = Rig.RForearm_Pole;
        }
        
        public void LateUpdate()
        {
            if (Rig == null) return;

            HandleBodyRotation();
            HandleHeadRotation();
            HandleArms();
        }

        private void HandleBodyRotation()
        {
            Rig.Root.position = Vars.MainCamera.transform.position;
            if ((Vars.MainCamera.transform.rotation.eulerAngles.y - Rig.Abdomen.rotation.eulerAngles.y) >= Quaternion.Euler(0, 90, 0).y
            || (Vars.MainCamera.transform.rotation.eulerAngles.y - Rig.Abdomen.rotation.eulerAngles.y) <= Quaternion.Euler(0, -90, 0).y)
            {
                Quaternion Rotation = Quaternion.Lerp(Rig.Abdomen.rotation, Vars.MainCamera.transform.rotation, Time.deltaTime * 5);
                Rig.Root.rotation = Quaternion.Euler(0, Rotation.eulerAngles.y, 0);
            }
        }
        private void HandleHeadRotation()
        {
            Rig.Head.position = Vars.MainCamera.transform.position;
            Rig.Head.rotation = Vars.MainCamera.transform.rotation;
        }

        private void HandleArms()
        {
            // main menu
            if (Vars.IsMainMenu)
            {
                Rig.LFeedbacker.Hand.Root.localScale = Vector3.one;
                Rig.LKnuckleblaster.Hand.Root.localScale = Vector3.one;
                Rig.LWhiplash.Hand.Root.localScale = Vector3.one;
                Rig.RFeedbacker.Hand.Root.localScale = Vector3.one;
                Rig.RSandboxer.Hand.Root.localScale = Vector3.one;
            }
            else
            {
                Rig.LFeedbacker.Hand.Root.localScale = Vector3.zero;
                Rig.LKnuckleblaster.Hand.Root.localScale = Vector3.zero;
                Rig.LWhiplash.Hand.Root.localScale = Vector3.zero;
                Rig.RFeedbacker.Hand.Root.localScale = Vector3.zero;
                Rig.RSandboxer.Hand.Root.localScale = Vector3.zero;
            }
            // sandbox arm
            Sandbox.Arm.SandboxArm[] Things = FindObjectsOfType<Sandbox.Arm.SandboxArm>();
            if (Things.Length != 0 && Sandbox.Arm.SandboxArm.Instance != null && Sandbox.Arm.SandboxArm.Instance.currentMode != null)
            {
                Rig.RFeedbacker.GameObjecT.localScale = Vector3.zero;
                Rig.RSandboxer.GameObjecT.localScale = RArmScale;
            }
            else
            {
                Rig.RFeedbacker.GameObjecT.localScale = RArmScale;
                Rig.RSandboxer.GameObjecT.localScale = Vector3.zero;
            }

            // arm swap
            foreach (Punch P in FindObjectsOfType<Punch>())
                if (P != null && P.enabled && P.gameObject.activeSelf)
                    switch(P.type)
                    {
                        case FistType.Standard:
                            {
                                Rig.LFeedbacker.GameObjecT.localScale = LFBArmScale;
                                Rig.LKnuckleblaster.GameObjecT.localScale = Vector3.zero;
                                break;
                            }
                        case FistType.Heavy:
                            {
                                Rig.LFeedbacker.GameObjecT.localScale = Vector3.zero;
                                Rig.LKnuckleblaster.GameObjecT.localScale = LArmScale;
                                break;
                            }
                        case FistType.Spear:
                        default: break;
                    }
            if (HookArm.Instance != null && HookArm.Instance.enabled && HookArm.Instance.model.activeSelf)
            {
                Rig.LFeedbacker.GameObjecT.localScale = Vector3.zero;
                Rig.LWhiplash.GameObjecT.localScale = LArmScale;
                Rig.LKnuckleblaster.GameObjecT.localScale = Vector3.zero;
            }
            else Rig.LWhiplash.GameObjecT.localScale = Vector3.zero;
        }
    }
}
