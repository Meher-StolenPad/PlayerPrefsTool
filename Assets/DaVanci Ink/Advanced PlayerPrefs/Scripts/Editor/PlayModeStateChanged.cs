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
            AdvancedPlayerPrefsSettings m_EncryptionSettings;
            switch (state)
            {
                case PlayModeStateChange.ExitingPlayMode:

                    if (AdvancedPlayerPrefs.TryGetEncryptionSettings(out m_EncryptionSettings))
                        m_EncryptionSettings.GetOldKeys();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    if (AdvancedPlayerPrefs.TryGetEncryptionSettings(out m_EncryptionSettings))
                        m_EncryptionSettings.SaveOldKeys();
                    break;
            }
        }
    }

}
