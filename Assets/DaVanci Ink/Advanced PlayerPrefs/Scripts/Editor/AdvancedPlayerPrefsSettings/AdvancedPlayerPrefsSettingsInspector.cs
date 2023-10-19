using Codice.Utils;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    [CustomEditor(typeof(AdvancedPlayerPrefsSettings))]
    [CanEditMultipleObjects]
    internal class AdvancedPlayerPrefsSettingsInspector : Editor
    {
        private AdvancedPlayerPrefsSettings APPSettings;
        private static bool DisplaySetKeys
        {
            get => EditorPrefs.GetBool(nameof(AdvancedPlayerPrefsSettingsInspector) + "." + nameof(DisplaySetKeys));
            set => EditorPrefs.SetBool(nameof(AdvancedPlayerPrefsSettingsInspector) + "." + nameof(DisplaySetKeys), value);
        }
        private static bool DisplayRuntimeSettings
        {   
            get => EditorPrefs.GetBool(nameof(AdvancedPlayerPrefsSettingsInspector) + "." + nameof(DisplayRuntimeSettings));
            set => EditorPrefs.SetBool(nameof(AdvancedPlayerPrefsSettingsInspector) + "." + nameof(DisplayRuntimeSettings), value);
        }
        private bool UseDeviceKey
        {
            get => APPSettings.useDeviceKey;
            set
            {
                APPSettings.useDeviceKey = value;
                if (!EditorUtility.IsDirty(APPSettings)) EditorUtility.SetDirty(APPSettings);
            }
        }
        private bool AutoEncryption 
        {
            get => APPSettings.AutoEncryption;
            set
            {
                APPSettings.AutoEncryption = value;
                if (!EditorUtility.IsDirty(APPSettings)) EditorUtility.SetDirty(APPSettings);
            }
        }
        private DebugMode _DebugMode
        {
            get => APPSettings.debugMode;    
            set
            {
                APPSettings.debugMode = value;
                if (!EditorUtility.IsDirty(APPSettings)) EditorUtility.SetDirty(APPSettings);
            }
        }
        private BackupMode _BackupMode   
        {
            get => APPSettings.backupMode;
            set
            {
                if (APPSettings.backupMode != value)
                {
                    APPSettings.backupMode = value;
                    APPSettings.OnBackupModeChanged();
                }
                else
                {
                    APPSettings.backupMode = value;
                }
                if (!EditorUtility.IsDirty(APPSettings)) EditorUtility.SetDirty(APPSettings);
            }
        }
        private string Key = string.Empty;
        private bool ShowErrorText => Key.Length == 32;
        private GUIStyle Textstyle;
        private GUIStyle Intstyle;
        private string LogError = "Key must be 32 bytes and Iv must be 16 bytes";
        private bool ShowKeys;

        private void OnEnable()
        {
            APPSettings = (AdvancedPlayerPrefsSettings)target;
            Key = APPSettings.GetKey();
        }
        public override void OnInspectorGUI()
        {
            Textstyle = new GUIStyle(EditorStyles.boldLabel);
            Textstyle.normal.textColor = Color.red;
            Intstyle = new GUIStyle(EditorStyles.miniLabel);
            float buttonWidth = (EditorGUIUtility.currentViewWidth - 10) / 2f;
            float newbuttonWidth = (EditorGUIUtility.currentViewWidth - 40) / 3f;

            GUI.enabled = false;
            base.OnInspectorGUI();
            GUILayout.Space(10);
          
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh Keys", GUILayout.Width(newbuttonWidth)))
            {
                int dialogResult = EditorUtility.DisplayDialogComplex(
    "All Player Prefs encrypted will be unencryptable!",
    "Do you want to create a backup file for your current keys before proceeding?",
    "Yes", "Don't Create", "Cancel");

                switch (dialogResult)
                {
                    case 0: //Create backup
                        APPSettings.ExportKeys();
                        APPSettings.RefreshKeys();
                        EditorUtility.SetDirty(APPSettings);
                        AssetDatabase.SaveAssets();
                        break;
                    case 1: //Don't create a backup
                        APPSettings.RefreshKeys();
                        EditorUtility.SetDirty(APPSettings);
                        AssetDatabase.SaveAssets();
                        break;
                    case 2: //Cancel process (Basically do nothing for now.)
                        break;
                    default:
                        Debug.LogWarning("Something went wrong when refreshing keys");
                        break;
                }
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Export Keys", GUILayout.Width(newbuttonWidth)))
            {
                APPSettings.ExportKeys();
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Generate Key", GUILayout.Width(newbuttonWidth)))
            {
                APPSettings.SetSavedKeyFromKeys();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            DisplaySetKeys = EditorGUILayout.BeginFoldoutHeaderGroup(DisplaySetKeys, "Set Keys");

            GUILayout.Space(10);

            if (DisplaySetKeys)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Key", GUILayout.Width(buttonWidth * 0.1f));
                Key = GUILayout.TextArea(Key, EditorStyles.textArea, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.Width(buttonWidth * 0.9f));
                if(Key.Length != 32)
                {
                    Intstyle.normal.textColor = Color.red;
                }
                else
                {
                    Intstyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                }
                EditorGUILayout.IntField(Key.Length,Intstyle, GUILayout.Width(buttonWidth * 0.1f));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);

                GUI.enabled = ShowErrorText;
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Set Keys", GUILayout.Width(buttonWidth)))
                {
                    if (ShowErrorText)
                    {
                        int dialogResult = EditorUtility.DisplayDialogComplex(
 "All Player Prefs encrypted will be unencryptable!",
 "Do you want to create a backup file for your current keys before proceeding?",
 "Yes", "Don't Create", "Cancel");

                        switch (dialogResult)
                        {
                            case 0: //Create backup
                                APPSettings.ExportKeys();
                                APPSettings.SetKeys(Key);
                                EditorUtility.SetDirty(APPSettings);
                                AssetDatabase.SaveAssets();
                                break;
                            case 1: //Don't create a backup
                                APPSettings.SetKeys(Key);
                                EditorUtility.SetDirty(APPSettings);
                                AssetDatabase.SaveAssets();
                                break;
                            case 2: //Cancel process (Basically do nothing for now.)
                                break;
                            default:
                                Debug.LogWarning("Something went wrong when refreshing keys");
                                break;
                        }
                    }
                }
                GUI.enabled = true;

                if (GUILayout.Button("Import Keys", GUILayout.Width(buttonWidth)))
                {
                   Key= APPSettings.ReadBackupFile();
                }
                EditorGUILayout.EndHorizontal();


                if (!ShowErrorText)
                {
                    GUILayout.Space(10);
                    GUILayout.Label(LogError, Textstyle, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.Width(buttonWidth * 1.8f));
                    GUILayout.Space(10);
                }

                EditorGUILayout.EndVertical();

            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(10);

            GUIContent toggleContent = new GUIContent("Use Auto-Encryption", "If true,all the player prefs will be saved with encryption settings set bellow\n" +
                "You don't need to set the encryption to true on the SetPrefs call.\n" +
                "See documentation to get more.");
            AutoEncryption = EditorGUILayout.Toggle(toggleContent, AutoEncryption, GUILayout.Width(buttonWidth * 4f));
            GUILayout.Space(10);

            string howToUse = "ATTENTION:\n" +
                   "When you change your current key or iv, you will lose all the encrypted data.\n" +
                   "Make sure that before you change the encryption settings, you create a backup of your old keys.\n" +
                   "Alternatively, use the Advanced PlayerPrefs tool to decrypt all your player prefs and create a backup file.\n" +
                   "You can then upload your player prefs settings again and encrypt them with the new keys.\n";

            EditorGUILayout.HelpBox(howToUse, MessageType.Info);


            DrawHorizontalLine(Color.grey);

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Debug Mode", GUILayout.Width(buttonWidth / 2.5f));
            _DebugMode = (DebugMode)EditorGUILayout.Popup((int)_DebugMode, AdvancedPlayerPrefsGlobalVariables.DebugMode, GUILayout.Width(buttonWidth / 1.1f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Backup Mode", GUILayout.Width(buttonWidth / 2.5f));
            _BackupMode = (BackupMode)EditorGUILayout.Popup((int)_BackupMode, AdvancedPlayerPrefsGlobalVariables.BackupMode, GUILayout.Width(buttonWidth / 1.1f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox("When you change the Product name or the Company Name on the Player Settings, you will lose all your saved PlayerPrefs. \n" +
                            "If Backup Mode is set to Auto-Update, all your PlayerPrefs will be automatically moved to the new project.", MessageType.Info);

            EditorGUILayout.EndVertical();


            EditorGUILayout.Space(5);

            DrawHorizontalLine(Color.grey);

            DisplayRuntimeSettings = EditorGUILayout.BeginFoldoutHeaderGroup(DisplayRuntimeSettings, "Runtime Settings");

            if (DisplayRuntimeSettings)
            {
                UseDeviceKey = EditorGUILayout.Toggle("Use Device Key", UseDeviceKey, GUILayout.Width(buttonWidth * 4f));
               
            }
        }
       
        private void DrawHorizontalLine(Color color)
        {
            var horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }

    }
}