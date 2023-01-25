using UnityEditor;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    [InitializeOnLoadAttribute]
    public class PlayModeStateChanged
    {
        static PlayModeStateChanged()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            ResetData(state);
        }

        private static void ResetData(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingPlayMode:

                    if (AdvancedPlayerPrefsSettings.Instance != null)
                        AdvancedPlayerPrefsSettings.Instance.GetOldKeys();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    if (AdvancedPlayerPrefsSettings.Instance != null)
                        AdvancedPlayerPrefsSettings.Instance.SaveOldKeys();
                    break;
            }
        }
    }

}
