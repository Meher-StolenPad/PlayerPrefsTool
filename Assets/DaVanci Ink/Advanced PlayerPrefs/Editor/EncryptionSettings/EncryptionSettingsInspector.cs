using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    [CustomEditor(typeof(EncryptionSettings))]
    [CanEditMultipleObjects]
    public class EncryptionSettingsInspector : Editor
    {
        private EncryptionSettings encryptionSetting;
        private static bool DisplaySetKeys
        {
            get => EditorPrefs.GetBool(nameof(EncryptionSettingsInspector) + "." + nameof(DisplaySetKeys));
            set => EditorPrefs.SetBool(nameof(EncryptionSettingsInspector) + "." + nameof(DisplaySetKeys), value);
        }
        private string Key = string.Empty;
        private string Lv = string.Empty;
        private bool ShowErrorText => Key.Length == 32 && Lv.Length == 16;
        private GUIStyle Textstyle;
        private GUIStyle Intstyle;  
        private string LogError = "Key must be in 32 byte and Lv must be in 16 byte";

        private void OnEnable()
        {
            encryptionSetting = (EncryptionSettings)target;
        }
        public override void OnInspectorGUI()
        {
            Textstyle = new GUIStyle(EditorStyles.boldLabel);
            Textstyle.normal.textColor = Color.red;
            Intstyle = new GUIStyle(EditorStyles.miniLabel);

            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            float buttonWidth = (EditorGUIUtility.currentViewWidth - 10) / 2f;

            if (GUILayout.Button("Refresh Keys", GUILayout.Width(buttonWidth)))
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
                        break;
                    case 1: //Don't create a backup
                        encryptionSetting.RefreshKeys();
                        break;
                    case 2: //Cancel process (Basically do nothing for now.)
                        break;
                    default:
                        Debug.LogWarning("Something went wrong when refreshing keys");
                        break;
                }
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Export Keys", GUILayout.Width(buttonWidth)))
            {
                encryptionSetting.ExportKeys();
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

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Lv",GUILayout.Width(buttonWidth * 0.1f));
                Lv = GUILayout.TextArea(Lv, EditorStyles.textArea, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.Width(buttonWidth * 0.9f));
                if (Lv.Length != 16)
                {
                    Intstyle.normal.textColor = Color.red;
                }
                else
                {
                    Intstyle.normal.textColor = Color.white;

                }
                EditorGUILayout.IntField(Lv.Length,Intstyle, GUILayout.Width(buttonWidth * 0.1f));
                EditorGUILayout.EndHorizontal();
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
                                encryptionSetting.SetKeys(Key, Lv);
                                break;
                            case 1: //Don't create a backup
                                encryptionSetting.SetKeys(Key, Lv);

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
            string howToUse = "HOW TO USE:  \n " +
                "ATTENTION : \n" +
                "- When you change your current key or Lv,you will lose all the encrypted date \n" +
                "- Make Sure before you change the encryption settings to create a backup for your old keys \n" +
                "  Or use the Advanced playerPrefs tool to Decrypte all your player Prefs,and create a backup file \n" +
                "2- You can upload your playerPrefs settings again and encrypte them with the new keys \n"
                ;
            EditorGUILayout.HelpBox(howToUse, MessageType.Info);
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
                Debug.Log(KeysExporter.Key);
                Key = KeysExporter.Key;
                Lv = KeysExporter.Lv;

            }
            catch (Exception e)
            {

            }
           

        }
    }
}