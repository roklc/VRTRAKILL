﻿using System.IO; using System.Diagnostics;
using BepInEx; using BepInEx.Logging;
using Valve.VR;

using Plugin.Helpers; using Plugin.VRTRAKILL;
using System.Reflection;

namespace Plugin
{
    /* If you're reading this and don't plan on stopping, then
     * welcome to the codebase where all hopes and dreams go die.
     * This is a dumpster fire of spaghetti code, inconsistent
     * naming and questionable life choices. Amen. */
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)] public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.whateverusername0.vrtrakill",
                            PLUGIN_NAME = "VRTRAKILL",
                            PLUGIN_VERSION = "0.9";

        public static string PluginPath = Assembly.GetExecutingAssembly().CodeBase,
                             FullGamePath = Process.GetCurrentProcess().MainModule.FileName,
                             GamePath = Path.GetDirectoryName(FullGamePath),
                             HMDModel = string.Empty;

        public static Patcher MainPatcher, HapticsPatcher, GunsPatcher, ArmsPatcher, IKPatcher;

        internal static ManualLogSource PLogger { get; private set; }

        public void Awake()
        {
            PLogger = Logger;

            VRTRAKILL.Config.ConfigMaster.Init();
            PatchStuff();
            VRTRAKILL.UI.UIConverter.Init();

            InitializeSteamVR();
        }
        private void PatchStuff()
        {
            System.Collections.Generic.List<string> Namespaces = new System.Collections.Generic.List<string>
            {
                typeof(Helpers.Patches.A).Namespace,

                typeof(VRTRAKILL.VRPlayer.VRCamera.Patches.A).Namespace,
                typeof(VRTRAKILL.UI.Patches.A).Namespace,
                typeof(VRTRAKILL.VRPlayer.Movement.Patches.A).Namespace,
            };
            if (Vars.Config.Input.InputSettings.EnableControllerHaptics)
                Namespaces.Add(typeof(VRTRAKILL.VRPlayer.Controllers.Patches.A).Namespace);
            if (Vars.Config.Game.CBS.EnableControllerShooting)
                Namespaces.Add(typeof(VRTRAKILL.VRPlayer.Guns.Patches.A).Namespace);
            if (Vars.Config.Game.MBP.EnableMovementPunching)
                Namespaces.Add(typeof(VRTRAKILL.VRPlayer.Arms.Patches.A).Namespace);
            if (!Vars.Config.Game.MBP.DisableControllerAiming)
                Namespaces.Add(typeof(VRTRAKILL.VRPlayer.Arms.Patches.Whiplash.A).Namespace);
            if (Vars.Config.Controllers.HandS.EnableVRIK)
                Namespaces.Add(typeof(VRTRAKILL.VRPlayer.Arms.VRIK.Patches.A).Namespace);

            MainPatcher = new Patcher(new HarmonyLib.Harmony($"{PLUGIN_GUID}.base"), _Namespaces: Namespaces.ToArray());
            MainPatcher.PatchAll();
        }
        private void InitializeSteamVR()
        {
            SteamVR_Actions.PreInitialize();
            SteamVR.Initialize();

            VRTRAKILL.Input.VRActionsManager.Init();
        }
    }
}