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
        internal const string InitialKey = "J5ddq99RUNJawf2QTGfQhtOjaSZ87h5g";
        internal const string InitialIv = "t9lWGXbFwmuqsxm1";

        internal const string GetStartedLink = "www.google.com";
        internal const string WebsiteLink = "www.google.com";
        internal const string ChangeLogsLink = "www.google.com";
        internal const string DocumentationLink = "www.google.com";
        internal const string DavanciInkLink = "www.google.com";

        internal const string NoSettingsWarning = "No Adavanced Player prefs settings Founded in the project ! Prefs will be saved and loaded without encryption." +
           "Check the Advanced Player Prefs Setup Panel,or the Adavanced Player prefs tool to create settings file.";
        internal const string NoEncryptionSettingsWarning = "No Adavanced Player prefs settings Founded in the project ! Prefs will be saved without encryption." +
            "Check the Advanced Player Prefs Setup Panel.";
        internal const string NoDecryptionSettingsWarning = "No Adavanced Player prefs settings Founded in the project ! Prefs will be loaded without decryption." +
            "Check the Advanced Player Prefs Setup Panel.";
        internal const  string EncryptionSettingsPath = "Assets/Resources/AdvancedPlayerPrefs/";
        internal const  string EncryptionSettingsResourcesPath = "AdvancedPlayerPrefs/Advanced Player Prefs Settings";
        internal const  string EncryptionSettingsFileName = "Advanced Player Prefs Settings.asset";


        internal const  string AdvancedPlayerPrefsToolMenuName = "DavanciCode/Advanced Player Prefs Tool %e";
        internal const  string AdvancedPlayerPrefsToolTitle = "Advanced Player Prefs Tool";

        internal const  string RefreshButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/refresh_Icon.png";
        internal const  string SaveButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/save_Icon.png";
        internal const  string RevertButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/reset_Icon.png";
        internal const  string DeleteButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/delete_Icon.png";
        internal const  string ApplyAllButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/apply_Icon.png";
        internal const  string ExportButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/d_popout_icon.png";

        internal const  string ToolbarSeachTextField = "ToolbarSeachTextField";
        internal const  string ToolbarSearchCancelButton = "ToolbarSeachCancelButton";

        internal static readonly string[] OpenFolderFilters = new string[] { "text files", "txt", "All files", "*" };

        internal static readonly string[] EnumList = new string[]
        {
            "Int",
            "Float",
            "String"
        };

        internal static readonly Color ShowAdvancedPlayerPrefsButtonColor = new Color32(255, 109, 2, 255);
        internal static readonly Color ShowAdvancedPlayerPrefsTextColor = new Color32(255, 216, 116, 255);
        internal static readonly Color ShowAdvancedPlayerPrefsTextColorNormal = new Color32(215, 66, 66, 255);

        internal static readonly Color SetupButtonTextColor = new Color32(101, 217, 255, 255);
        internal static readonly Color SetupButtonColor = new Color32(0, 157, 255, 255);

        internal static readonly Color NormalEncryptedTextColor = new Color32(0, 90, 255, 255);
        internal static readonly Color ProEncryptedTextColor = new Color32(101, 217, 255, 255);

        internal static readonly Color NormalSearchTextColor = new Color32(245, 121, 47, 255);
        internal static readonly Color ProSearchTextColor = new Color32(255, 216, 116, 255);

        internal static readonly Color NormalChangedTextColor = new Color32(6, 140, 6, 255);
        internal static readonly Color ProChangedTextColor = new Color32(0, 255, 0, 255);

        internal static readonly Color GreenDebugColor = new Color32(67, 142,83, 255);
        internal static readonly Color ProGreenDebugColor = new Color32(0, 255, 0, 255);

        internal static readonly Color CyanDebugColor = new Color32(0, 33, 255, 255);
        internal static readonly Color ProCyanDebugColor = new Color32(0, 255, 255, 255);

        internal static readonly Color RedDebugColor = new Color32(255, 0, 0, 255);
        internal static readonly Color ProRedDebugColor = new Color32(255, 0, 0, 255);

        internal static readonly Color GreyDebugColor = new Color32(0, 255, 255, 255);
        internal static readonly Color ProGreyDebugColor = new Color32(128, 128, 128, 255);

        internal static readonly Color YellowebugColor = new Color32(150, 0, 112, 255);
        internal static readonly Color ProYellowDebugColor = new Color32(255, 235, 4, 255);

#if UNITY_EDITOR        
        internal static string GetPlayerPrefsSpecificText()
        {
           return BackupCreatedText+ "//Keys for product  : " + Application.productName + " , Company :  " + Application.companyName + '\n'
               + "//Created at : " + DateTime.Now + "\n//Created by " + UnityEditor.CloudProjectSettings.userName + '\n';
        }
#endif
    }
}