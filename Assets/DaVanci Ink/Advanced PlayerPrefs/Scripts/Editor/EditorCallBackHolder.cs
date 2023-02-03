using UnityEditor;
using UnityEngine;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    public static class EditorCallBackHolder
    {
        private static string previousProductName;
        private static string previousCompanyName;

        private static string CurrentCompanyName;
        private static string CurrentProductName;

        private static float LastCheckTime;
        private static bool Subscribed;
        private static bool InProgress;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Debug.Log("Initialize");
            AdvancedPlayerPrefs.OnSettingsCreated += Initialize;
            AdvancedPlayerPrefsSettings.OnBackupModeChangedAction += BackupModeChanged;
            previousProductName = PlayerSettings.productName;
            previousCompanyName = PlayerSettings.companyName;

            if (AdvancedPlayerPrefsSettings.Instance != null)
            {
                if (AdvancedPlayerPrefsSettings.Instance.backupMode != BackupMode.Disable)
                {
                    EditorApplication.update += CheckForProductNameChange;
                    Subscribed = true;
                }
            }
        }
        private static void BackupModeChanged()
        {

            if (AdvancedPlayerPrefsSettings.Instance.backupMode == BackupMode.Disable)
            {
                if (Subscribed)
                {
                    EditorApplication.update -= CheckForProductNameChange;
                    Subscribed = false;
                }
            }
            else
            {
                if (!Subscribed)
                {
                    EditorApplication.update += CheckForProductNameChange;
                    Subscribed = true;
                }
            }  
        }
        private static void CheckForProductNameChange()
        {
            if (AdvancedPlayerPrefsSettings.Instance == null)
            {
                EditorApplication.update -= CheckForProductNameChange;
            }

            float currentTime = Time.realtimeSinceStartup;

            if (currentTime - LastCheckTime >= 6.0f)
            {
                CurrentProductName = PlayerSettings.productName;
                CurrentCompanyName = PlayerSettings.companyName;

                if (!previousProductName.Equals(CurrentProductName) || !previousCompanyName.Equals(CurrentCompanyName))
                {
                    if (AdvancedPlayerPrefsSettings.Instance.backupMode == BackupMode.Manual_Update)
                    {
                        if (AdvancedPlayerPrefsCopierPanel.IsActive)
                        {
                            Debug.Log("Upddate info there");
                        }
                        else
                        {
                            AdvancedPlayerPrefsCopierPanel.ShowWindow();
                        }
                    }
                    else
                    {
                        if (!InProgress)
                        {
                            InProgress = true;
                            AdvancedPlayerPrefsTool.ImportFrom(previousCompanyName, previousProductName);
                            previousCompanyName = CurrentCompanyName;
                            previousProductName = CurrentProductName;
                            Debug.Log("Prefs Auto Upadted : " + CurrentProductName + "/" + CurrentProductName);
                            InProgress = false;
                        }



                    }
                    Debug.Log("Product/Company name changed to: " + CurrentProductName + "/" + CurrentProductName);
                }
                LastCheckTime = currentTime;
            }
        }
        internal static void UpadteInfo()
        {
            previousCompanyName = CurrentCompanyName;
            previousProductName = CurrentProductName;
        }
    }
}
