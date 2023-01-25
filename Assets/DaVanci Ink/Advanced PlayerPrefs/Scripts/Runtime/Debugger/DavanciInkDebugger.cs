using UnityEngine;
using Color = UnityEngine.Color;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal static class DavanciDebug
    {
        internal static DebugMode dm = DebugMode.EditorOnly;

        internal static void Log(string message, Color color)
        {
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

        private static void CheckNewSettings()
        {
            if (AdvancedPlayerPrefsSettings.Instance != null && dm != AdvancedPlayerPrefsSettings.Instance.debugMode)
                dm = AdvancedPlayerPrefsSettings.Instance.debugMode;
        }

        public static void Warning(string message)
        {
            CheckNewSettings();
            switch (dm)
            {
                case DebugMode.EditorOnly:
#if UNITY_EDITOR
                    Debug.LogWarning("<color=#" + FromColor(Color.yellow) + ">" + message + "</color>");
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
