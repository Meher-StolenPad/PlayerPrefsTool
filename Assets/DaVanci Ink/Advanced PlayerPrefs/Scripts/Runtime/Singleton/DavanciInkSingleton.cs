using UnityEngine;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal abstract class DavanciInkSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        static T instance;
        static bool Loaded = false;

        internal static T Instance
        {
            get
            {
                if (Loaded) return instance;

                if (instance == null)
                {
                    instance = Resources.Load<T>(AdvancedPlayerPrefsGlobalVariables.EncryptionSettingsResourcesPath);
                    if (instance != null)
                        (instance as DavanciInkSingleton<T>).OnInitialize();
                }
                Loaded = true;

                return instance;
            }
        }
        internal static T Reload()
        {
            instance = Resources.Load<T>(AdvancedPlayerPrefsGlobalVariables.EncryptionSettingsResourcesPath);
            if (instance != null)
                (instance as DavanciInkSingleton<T>).OnInitialize();
            return instance;
        }
        internal virtual void OnInitialize() { }
    }

}
