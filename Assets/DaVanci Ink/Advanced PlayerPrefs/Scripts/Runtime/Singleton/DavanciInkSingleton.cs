using UnityEngine;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    public abstract class DavanciInkSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<T>(AdvancedPlayerPrefsGlobalVariables.EncryptionSettingsResourcesPath);
                    if (instance != null)
                        (instance as DavanciInkSingleton<T>).OnInitialize();
                }
                return instance;
            }
        }
        public static T Reload()
        {
            instance = Resources.Load<T>(AdvancedPlayerPrefsGlobalVariables.EncryptionSettingsResourcesPath);
            (instance as DavanciInkSingleton<T>).OnInitialize();
            return instance;
        }
        protected virtual void OnInitialize() { }
    }

}
