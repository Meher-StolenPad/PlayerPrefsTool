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

        private static string HolderCompanyName;
        private static string HolderProductName;    

        private static float LastCheckTime;
        private static bool Subscribed;
        private static bool InProgress;
        private static float LastChangeTime;

            
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
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

                if (!InProgress)
                {
                    if (HolderCompanyName != CurrentCompanyName || HolderProductName != CurrentProductName)
                    {
                        HolderCompanyName = CurrentCompanyName;
                        HolderProductName = CurrentProductName;
                        LastChangeTime = currentTime;
                    }
                    if (currentTime - LastChangeTime >= 5f)
                    {
                        CheckChanges(currentTime);
                    }

                }
                
                LastCheckTime = currentTime;
            }
        }

        private static void CheckChanges(float currentTime)
        {

            if (!previousProductName.Equals(CurrentProductName) || !previousCompanyName.Equals(CurrentCompanyName))
            {
                if (AdvancedPlayerPrefsSettings.Instance.backupMode == BackupMode.Manual_Update)
                {
                    int playerPrefsCount = AdvancedPlayerPrefsTool.GetPlayerPrefsCount(previousCompanyName, previousProductName);
                    if (playerPrefsCount > 0)
                    {
                        if (AdvancedPlayerPrefsCopierPanel.IsActive)
                        {
                        }
                        else
                        {
                            AdvancedPlayerPrefsCopierPanel.Init(playerPrefsCount, previousCompanyName,previousProductName);
                            AdvancedPlayerPrefsCopierPanel.ShowWindow();
                            LastChangeTime = currentTime;
                        }
                    }
                    else
                    {

                        previousCompanyName = CurrentCompanyName;
                        previousProductName = CurrentProductName;
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
                        LastChangeTime = currentTime;
                    }
                }
            }
        }

        internal static void UpadteInfo()
        {
            previousCompanyName = CurrentCompanyName;
            previousProductName = CurrentProductName;
            HolderCompanyName = CurrentCompanyName;
            HolderProductName = CurrentProductName;
        }
    }
}
