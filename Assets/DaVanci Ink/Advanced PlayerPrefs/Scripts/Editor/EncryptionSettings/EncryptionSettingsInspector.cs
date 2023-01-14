using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    [CustomEditor(typeof(EncryptionSettings))]
    [CanEditMultipleObjects]
    internal class EncryptionSettingsInspector : Editor
    {
        private EncryptionSettings encryptionSetting;
        private static bool DisplaySetKeys
        {
            get => EditorPrefs.GetBool(nameof(EncryptionSettingsInspector) + "." + nameof(DisplaySetKeys));
            set => EditorPrefs.SetBool(nameof(EncryptionSettingsInspector) + "." + nameof(DisplaySetKeys), value);
        }
        private static bool DisplayRuntimeSettings
        {   
            get => EditorPrefs.GetBool(nameof(EncryptionSettingsInspector) + "." + nameof(DisplayRuntimeSettings));
            set => EditorPrefs.SetBool(nameof(EncryptionSettingsInspector) + "." + nameof(DisplayRuntimeSettings), value);
        }
        private bool UseDeviceKey
        {
            get => encryptionSetting.useDeviceKey;
            set
            {
                encryptionSetting.useDeviceKey = value;
                if (!EditorUtility.IsDirty(encryptionSetting)) EditorUtility.SetDirty(encryptionSetting);
            }
        }


        private string Key = string.Empty;
        private bool ShowErrorText => Key.Length == 32;
        private GUIStyle Textstyle;
        private GUIStyle Intstyle;  
        private string LogError = "Key must be in 32 byte and Iv must be in 16 byte";
        private bool ShowKeys;

        private void OnEnable()
        {
            encryptionSetting = (EncryptionSettings)target;
            Key = encryptionSetting.GetKey();
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
                  "All Player Prefs encrypted will be undercreptable !",
                  "Do you want to create a backup file for your current keys before proceed  ?",
                  "Yes", "Don't Create", "Cancel");

                switch (dialogResult)
                {
                    case 0: //Create backup
                        encryptionSetting.ExportKeys();
                        encryptionSetting.RefreshKeys();
                        EditorUtility.SetDirty(encryptionSetting);
                        AssetDatabase.SaveAssets();
                        break;
                    case 1: //Don't create a backup
                        encryptionSetting.RefreshKeys();
                        EditorUtility.SetDirty(encryptionSetting);
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
                encryptionSetting.ExportKeys();
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Generate Key", GUILayout.Width(newbuttonWidth)))
            {
                encryptionSetting.SetSavedKeyFromKeys();
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
                    Intstyle.normal.textColor = Color.white;

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
                 "All Player Prefs encrypted will be undercreptable !",
                 "Do you want to create a backup file for your current keys before proceed  ?",
                 "Yes", "Don't Create", "Cancel");

                        switch (dialogResult)
                        {
                            case 0: //Create backup
                                encryptionSetting.ExportKeys();
                                encryptionSetting.SetKeys(Key);
                                EditorUtility.SetDirty(encryptionSetting);
                                AssetDatabase.SaveAssets();
                                break;
                            case 1: //Don't create a backup
                                encryptionSetting.SetKeys(Key);
                                EditorUtility.SetDirty(encryptionSetting);
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
                   Key= encryptionSetting.ReadBackupFile();
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

            string howToUse = "ATTENTION :  \n " +
                "  When you change your current key or iv,you will lose all the encrypted data \n" +
                "  Make Sure before you change the encryption settings to create a backup for your old keys \n" +
                "  Or use the Advanced playerPrefs tool to Decrypte all your player Prefs,and create a backup file \n" +
                "  You can upload your playerPrefs settings again and encrypte them with the new keys \n"
                ;
            EditorGUILayout.HelpBox(howToUse, MessageType.Info);

            DrawHorizontalLine(Color.grey);
            DisplayRuntimeSettings = EditorGUILayout.BeginFoldoutHeaderGroup(DisplayRuntimeSettings, "Runtime Settings");

            if (DisplayRuntimeSettings)
            {
                UseDeviceKey = EditorGUILayout.ToggleLeft("Use Device Key", UseDeviceKey, GUILayout.Width(buttonWidth * 4f));
               
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