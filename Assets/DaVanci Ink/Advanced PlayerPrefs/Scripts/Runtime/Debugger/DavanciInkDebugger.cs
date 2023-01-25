using UnityEngine;
using Color = UnityEngine.Color;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal static class DavanciDebug
    {
        internal static DebugMode dm = DebugMode.EditorOnly;

        private static bool isProMode = false;
        private static bool isProSet = false;

        static DavanciDebug()
        {
            bool t = IsProMode;
        }
        internal static bool IsProMode
        {
            get
            {
                if(isProMode == false && !isProSet)
                {
#if UNITY_EDITOR
                    isProMode = UnityEditor.EditorGUIUtility.isProSkin;
#endif
                }
                isProSet = true;

                return isProMode;
            }
        }

        private static void CheckNewSettings()
        {
            if (AdvancedPlayerPrefsSettings.Instance != null && dm != AdvancedPlayerPrefsSettings.Instance.debugMode)
                dm = AdvancedPlayerPrefsSettings.Instance.debugMode;
        }
        internal static void Log(string message, Color color)
        {
            if (color.Equals(Color.green))
            {
                color = IsProMode ? AdvancedPlayerPrefsGlobalVariables.ProGreenDebugColor : AdvancedPlayerPrefsGlobalVariables.GreenDebugColor;
            }
            else if (color.Equals(Color.cyan))
            {
                color = IsProMode ? AdvancedPlayerPrefsGlobalVariables.ProCyanDebugColor : AdvancedPlayerPrefsGlobalVariables.CyanDebugColor;
            }
            else if (color.Equals(Color.red))
            {
                color =IsProMode ? AdvancedPlayerPrefsGlobalVariables.ProRedDebugColor : AdvancedPlayerPrefsGlobalVariables.RedDebugColor;
            }
            else if (color.Equals(Color.grey))
            {
                color = IsProMode ? AdvancedPlayerPrefsGlobalVariables.ProGreyDebugColor : AdvancedPlayerPrefsGlobalVariables.GreyDebugColor;
            }
            CheckNewSettings();

            switch (dm)
            {
                case DebugMode.EditorOnly:
#if UNITY_EDITOR
                    Debug.Log("<color=#" + FromColor(color) + ">" + message + "</color>");
#endif
                    break;
                case DebugMode.Actif:
                    Debug.Log("<color=#" + FromColor(color) + ">" + message + "</color>");
                    break;
            }
        }
        public static void Warning(string message)
        {
            CheckNewSettings();
            switch (dm)
            {
                case DebugMode.EditorOnly:
#if UNITY_EDITOR
                    Debug.LogWarning("<color=#" + FromColor(isProMode ? AdvancedPlayerPrefsGlobalVariables.ProYellowDebugColor : AdvancedPlayerPrefsGlobalVariables.YellowebugColor) + ">" + message + "</color>");
#endif
                    break;
                case DebugMode.Actif:
                    Debug.LogWarning("<color=#" + FromColor(Color.yellow) + ">" + message + "</color>");
                    break;
            }
        }
        public static void Error(string message)
        {
            CheckNewSettings();
            switch (dm)
            {
                case DebugMode.EditorOnly:
#if UNITY_EDITOR
                    Debug.LogError("<color=#" + FromColor(Color.red) + ">" + message + "</color>");
#endif
                    break;
                case DebugMode.Actif:
                    Debug.LogError("<color=#" + FromColor(Color.red) + ">" + message + "</color>");
                    break;
            }
        }
        public static string FromColor(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
            // int r = (int)color.r, g = (int)color.g, b = (int)color.b;
            // return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
    }
}
