using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal static class AdvancedPlayerPrefsGlobalVariables
    {
        internal const string BackupCreatedText = "//File Create with Player prefs editor Tool\n";


        internal const string KeyBackupFileNamePath = "/PPKeysBackup.txt";
        internal const string KeyBackupFileName = "PPKeysBackup";
        internal const string KeyOpenPanelTitle = "Backup Keys path";

        internal const string CharsKey = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        internal const string InitialKey = "A60A5770FE5E7AB200BA9CFC94E4E8B0";
        internal const string InittialIv = "1234567887654321";
        internal const string InittialSavedKey = "A610A2573704FE55E67A7B28008BA79C6FC5944E43E82B01";
        internal const string CryptoSaveKey = "APPsCSDK";


        internal static Color ShowAdvancedPlayerPrefsButtonColor = new Color32(255, 109, 2, 255);
        internal static Color ShowAdvancedPlayerPrefsTextColor = new Color32(255, 216, 116, 255);

        internal static Color SetupButtonTextColor = new Color32(20, 180, 255, 255);
        internal static Color SetupButtonColor = new Color32(20, 89, 255, 255);
#if UNITY_EDITOR
        internal static string GetPlayerPrefsSpecificText()
        {

           return BackupCreatedText+ "//Keys for product  : " + Application.productName + " , Company :  " + Application.companyName + '\n'
               + "//Created at : " + DateTime.Now + "\n//Created by " + UnityEditor.CloudProjectSettings.userName + '\n';
        }
#endif
    }
}