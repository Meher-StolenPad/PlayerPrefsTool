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
            // encryptionSetting.SetSavedKeyFromKeys(encryptionSetting.GetKey(), encryptionSetting.Getiv());
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

                //EditorGUILayout.BeginHorizontal();
                //GUILayout.Label("Iv",GUILayout.Width(buttonWidth * 0.1f));
                //GUILayout.TextArea(encryptionSetting.Iv, EditorStyles.textArea, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.Width(buttonWidth * 0.9f));
              
                //EditorGUILayout.IntField(encryptionSetting.Iv.Length,Intstyle, GUILayout.Width(buttonWidth * 0.1f));
                //EditorGUILayout.EndHorizontal();
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
                    ReadBackupFile();
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
                encryptionSetting.useDeviceKey = EditorGUILayout.ToggleLeft("Use Device Key", encryptionSetting.useDeviceKey, GUILayout.Width(buttonWidth * 4f));
            }
        }
        private void ReadBackupFile()
        {
            try
            {
                string[] filters = new string[] { "text files", "txt", "All files", "*" };
                string path = EditorUtility.OpenFilePanelWithFilters("Load keys backup file", "", filters);

                if (string.IsNullOrEmpty(path)) return;

                var stringArray = File.ReadLines(path).Where(line => !line.StartsWith("//")).ToArray();
                var newString = string.Empty;

                foreach (var item in stringArray)
                {
                    newString += item;
                }

                KeysExporter KeysExporter = JsonUtility.FromJson<KeysExporter>(newString);
                Key = KeysExporter.Key;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
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