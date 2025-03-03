using System;
using System.Reflection;

using HarmonyLib;
using UnityEngine;
using UnityExplorer.UI.Panels;

#if MELONLOADER
using MelonLoader;
#endif

namespace UEIntegration.Patches {
#if BEPINEX
    [HarmonyPatch]
#endif
    static class FreecamDefaults {
        private static Type freeCamPanel;

#if BEPINEX
        static MethodBase TargetMethod() {
            freeCamPanel = AccessTools.TypeByName("UnityExplorer.UI.Panels.FreeCamPanel");
            return AccessTools.Method(freeCamPanel, "SetupFreeCamera");
        }

#elif MELONLOADER
        static bool hasPatched = false;

        public static void Patch(HarmonyLib.Harmony harmony) {
            if (hasPatched == true) {
                return;
            }

            freeCamPanel = AccessTools.TypeByName("UnityExplorer.UI.Panels.FreeCamPanel");
            MethodInfo setupFreeCamera = AccessTools.Method(freeCamPanel, "SetupFreeCamera");
            MethodInfo postfix = AccessTools.Method(typeof(FreecamDefaults), nameof(FreecamDefaults.Postfix));

            harmony.Patch(setupFreeCamera, null, postfix, null);
        }

#endif

        static void Postfix() {
            Config.Freecam config = Plugin.instance.config.freecam;

            Camera camera = (Camera) AccessTools.Field(freeCamPanel, "ourCamera")
                .GetValue(null);

            camera.farClipPlane = config.farClipPlane.Value;
            camera.fieldOfView = config.fieldOfView.Value;
        }
    }
}
