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
       
        private DebugMode DebugMode
        {
            get => APPSettings.debugMode;    
            set
            {
                APPSettings.debugMode = value;
                if (!EditorUtility.IsDirty(APPSettings)) EditorUtility.SetDirty(APPSettings);
            }
        }

        private string Key = string.Empty;
        private bool ShowErrorText => Key.Length == 32;
        private GUIStyle Textstyle;
        private GUIStyle Intstyle;  
        private readonly string LogError = "Key must be in 32 bits and Iv must be in 16 bits";
        private readonly bool ShowKeys;

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
            float newbuttonWidth = (EditorGUIUtility.currentViewWidth - 40) / 2f;

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
                 "All Player Prefs encrypted will be undercreptable !",
                 "Do you want to create a backup file for your current keys before proceed  ?",
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

            string howToUse = "ATTENTION :  \n " +
                "  When you change your current key or iv,you will lose all the encrypted data \n" +
                "  Make Sure before you change the encryption settings to create a backup for your old keys \n" +
                "  Or use the Advanced playerPrefs tool to Decrypte all your player Prefs,and create a backup file \n" +
                "  You can upload your playerPrefs settings again and encrypte them with the new keys \n"
                ;
            EditorGUILayout.HelpBox(howToUse, MessageType.Info);

            DrawHorizontalLine(Color.grey);
            EditorGUILayout.Space(5);
            DebugMode = (DebugMode)EditorGUILayout.EnumPopup("Debug Mode",DebugMode, GUILayout.Width(buttonWidth*1.5f));
            EditorGUILayout.Space(5);

            DrawHorizontalLine(Color.grey);
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