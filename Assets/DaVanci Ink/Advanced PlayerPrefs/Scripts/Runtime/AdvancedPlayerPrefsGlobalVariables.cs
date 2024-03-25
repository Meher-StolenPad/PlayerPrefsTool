using System;
using UnityEngine;
#if UNITY_EDITOR
#endif
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal static class AdvancedPlayerPrefsGlobalVariables
    {

        internal const string BackupCreatedText = "//This file was created with the PlayerPrefs Editor Tool.\n";


        internal const string KeyBackupFileNamePath = "/PPKeysBackup.txt";
        internal const string KeyBackupFileName = "PPKeysBackup";
        internal const string KeyOpenPanelTitle = "Backup Keys path";

        internal const string CharsKey = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        internal const string InitialKey = "i8rjckih86IYDf7phLLAxD67SDzVhsfy";
        internal const string InitialIv = "am0nTXiNOcjEwRZi";
        internal const string InitialSavedKey = "i8,arjsmck+0ihxn86cTIYPXDf8i7pLNhLUOLA1cxDej67kESDBwzVARhs_ZfyPi";

        internal const string GetStartedLink = "https://tools.davanciink.com/AdvancedPlayerPrefs/#get_started";
        internal const string WebsiteLink = "https://tools.davanciink.com/AdvancedPlayerPrefs";
        internal const string ChangeLogsLink = "www.google.com";
        internal const string DocumentationLink = "https://tools.davanciink.com/AdvancedPlayerPrefs/#doc";
        internal const string DavanciInkLink = "https://tools.davanciink.com/AdvancedPlayerPrefs";

        internal const string NoSettingsWarning = "No Advanced PlayerPrefs settings found in the project! Prefs will be saved and loaded without encryption." +
   "Check the Advanced PlayerPrefs Setup Panel or the Advanced PlayerPrefs Tool to create a settings file.";
        internal const string NoEncryptionSettingsWarning = "No Advanced Player Prefs settings found in the project! Prefs will be saved without encryption." +
            " Check the Advanced Player Prefs Setup Panel.";
        internal const string NoDecryptionSettingsWarning = "No Adavanced Player prefs settings Founded in the project ! Prefs will be loaded without decryption." +
            "Check the Advanced Player Prefs Setup Panel.";
        internal const string EncryptionSettingsPath = "Assets/Resources/AdvancedPlayerPrefs/";
        internal const string EncryptionSettingsResourcesPath = "AdvancedPlayerPrefs/Advanced Player Prefs Settings";
        internal const string EncryptionSettingsFileName = "Advanced Player Prefs Settings.asset";


        internal const string AdvancedPlayerPrefsToolMenuName = "Window/Advanced Player Prefs/Editor Tool %e";
        internal const string AdvancedPlayerPrefsToolTitle = "Advanced Player Prefs Tool";
        internal const string AdvancedPlayerPrefsSetupMenuName = "Window/Advanced Player Prefs/Setup";

        internal const string RefreshButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/refresh_Icon.png";
        internal const string SaveButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/save_Icon.png";
        internal const string RevertButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/reset_Icon.png";
        internal const string DeleteButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/delete_Icon.png";
        internal const string ApplyAllButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/apply_Icon.png";
        internal const string ExportButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/d_popout_icon.png";
        internal const string PinButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/pinIcon.png";
        internal const string UnpinButtonIconTexturePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/unpinIcon.png";

        internal const string ToolbarSeachTextField = "ToolbarSeachTextField";
        internal const string ToolbarSearchCancelButton = "ToolbarSeachCancelButton";
        internal const string ToolbarSeachTextField2021 = "ToolbarSearchTextField";
        internal const string ToolbarSearchCancelButton2021 = "ToolbarSearchCancelButton";

        internal static readonly string[] OpenFolderFilters = new string[] { "text files", "txt", "All files", "*" };

        internal static readonly string[] EnumList = new string[]
        {
            "Int",
            "Float",
            "String",
            "Byte"
        };
        internal static readonly string[] TypeList = new string[]
       {
            "Int",
            "Float",
            "String",
            "Byte"
       };
        internal static readonly string[] PathTypeList = new string[]
        {
            "Project",
            "Assets Folder",
            "Persistant Data",
            "Temporary Cache",
            "Absolute",

        };
        internal static readonly string[] DebugMode = new string[]
        {
            "Slient Mode",
            "Editor Only (Recommended)",
            "Build & Editor Mode"
        };
        internal static readonly string[] BackupMode = new string[]
        {
            "Auto-Update",
            "Manual Update (Recommended)",
            "Disable"
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

        internal static readonly Color GreenDebugColor = new Color32(67, 142, 83, 255);
        internal static readonly Color ProGreenDebugColor = new Color32(0, 255, 0, 255);

        internal static readonly Color CyanDebugColor = new Color32(0, 33, 255, 255);
        internal static readonly Color ProCyanDebugColor = new Color32(0, 255, 255, 255);

        internal static readonly Color RedDebugColor = new Color32(255, 0, 0, 255);
        internal static readonly Color ProRedDebugColor = new Color32(255, 0, 0, 255);

        internal static readonly Color GreyDebugColor = new Color32(0, 255, 255, 255);
        internal static readonly Color ProGreyDebugColor = new Color32(128, 128, 128, 255);

        internal static readonly Color YellowebugColor = new Color32(150, 0, 112, 255);
        internal static readonly Color ProYellowDebugColor = new Color32(255, 235, 4, 255);

        internal static readonly Color PinButtonColor = new Color32(15, 211, 255, 255);
        internal static readonly Color UnpinButtonColor = new Color32(70, 177, 201, 255);

        internal const string SamplesPackagePath = "Assets/DaVanci Ink/Advanced PlayerPrefs/Samples.unitypackage";


#if UNITY_EDITOR        
        internal static string GetPlayerPrefsSpecificText()
        {
            return BackupCreatedText +
                "//Product : " + Application.productName + '\n' +
                "//Company :  " + Application.companyName + '\n'
                + "//Created on : " + DateTime.Now +
                "\n//Created by : " + UnityEditor.CloudProjectSettings.userName + '\n';
        }
#endif
    }
}