﻿using UnityEngine;
using UnityEngine.UI;

namespace Plugin.VRTRAKILL.UI
{
    // mostly "borrowed" from huskvr
    internal class UIConverter
    {
        public static Camera UICamera { get; private set; }

        public static void ConvertAllCanvases()
        {
            UICamera = new GameObject("UI Camera").AddComponent<Camera>();
            UICamera.cullingMask = 1 << (int)Vars.Layers.UI;
            UICamera.clearFlags = CameraClearFlags.Depth; UICamera.depth = 1;

            if (!Vars.Config.UIInteraction.ControllerBased)
                UICamera.gameObject.AddComponent<UIInteraction>();

            foreach (Canvas C in Object.FindObjectsOfType<Canvas>())
                if (!Helpers.Misc.HasComponent<UICanvas>(C.gameObject))
                    RecursiveConvertCanvas();
        }

        private static void RecursiveConvertCanvas(GameObject GO = null)
        {
            if (GO != null)
            {
                try { ConvertCanvas(GO.GetComponent<Canvas>()); } catch {}

                if (GO.transform.childCount > 0)
                    for (int i = 0; i < GO.transform.childCount; i++)
                        RecursiveConvertCanvas(GO.transform.GetChild(i).gameObject);
            }
            else
            {
                foreach (Canvas C in Object.FindObjectsOfType<Canvas>())
                    if (!Helpers.Misc.HasComponent<UICanvas>(C.gameObject))
                        try { ConvertCanvas(C); } catch {}
            }
        }
        public static void ConvertCanvas(Canvas C, bool Force = false, bool DontAddComponent = false)
        {
            if (!Force)
            {
                if (C.renderMode != RenderMode.ScreenSpaceOverlay) return;
            }

            C.worldCamera = UICamera;
            C.renderMode = RenderMode.WorldSpace;
            C.gameObject.layer = (int)Vars.Layers.UI;
            if (!DontAddComponent) C.gameObject.AddComponent<UICanvas>();

            foreach (Transform Child in C.transform) ConvertElement(Child);
        }
        private static void ConvertElement(Transform Element)
        {
            if (Element.GetComponent<Selectable>() is Selectable Button)
            {
                ColorBlock block = Button.colors;
                block.highlightedColor = Color.red;
                Button.colors = block;
            }

            foreach (Transform Child in Element) ConvertElement(Child);
        }
    }
}
