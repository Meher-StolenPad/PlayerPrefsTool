using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static DaVanciInk.AdvancedPlayerPrefs.PlayerPrefsWindow;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal enum SavePathType : int
    {
        Project = 0,
        Assets = 1,
        Persistant =2,
        Temporary =3,
        Absolute =4
    }
    public static class AdvancedPlayerPrefsExportManager
    {
        internal static SavePathType m_savePathType;
        internal static string m_exportPath;
        internal static string GetPathWithType(SavePathType m_savePathType = SavePathType.Project)
        {
            string path = string.Empty;
            switch (m_savePathType)
            {
                case SavePathType.Project:
                    path= Application.dataPath.Replace("/Assets", "");
                    break;
                case SavePathType.Assets:
                    path = Application.dataPath;
                    break;
                case SavePathType.Persistant:
                    path = Application.persistentDataPath;
                    break;
                case SavePathType.Temporary:
                    path = Application.temporaryCachePath;
                    break;
            }
            return path;
        }
        internal static void ShowExplorer(string itemPath)
        {
            EditorUtility.RevealInFinder(itemPath);
        }
        internal static void Export(List<PlayerPrefHolder> PlayerPrefHolderList, string _exportPath, SavePathType _savePathType)
        {
            m_exportPath = _exportPath;
            if (_savePathType != SavePathType.Absolute)
                m_exportPath = GetPathWithType(_savePathType);

            var backupstring = CreateBackup(PlayerPrefHolderList);
            string newBackupString = PlayerPrefsGlobalVariables.CreatedText;
            string playerprefsSpecific = "//Player prefs for product  : " + Application.productName + " , Company :  " + Application.companyName + '\n'
                + "//Created at : " + DateTime.Now + "\n//Created by " + UnityEditor.CloudProjectSettings.userName + '\n';
            newBackupString += playerprefsSpecific;

            newBackupString += backupstring;

            //ExportPath = EditorUtility.OpenFolderPanel("Backup path", "", "PPbackup.txt");
            string ExportPathFile = m_exportPath + "/PPbackup.txt";
            if (!File.Exists(ExportPathFile))
            {
                File.WriteAllText(ExportPathFile, newBackupString);
            }
            else
            {
                ExportPathFile = AdvancedPlayerPrefs.NextAvailableFilename(ExportPathFile);
                File.WriteAllText(ExportPathFile, newBackupString);
            }
            Debug.Log(ExportPathFile);
        }

        internal static string CreateBackup(List<PlayerPrefHolder> PlayerPrefHolderList)
        {
            ExportSerialzerHolder exportSerialzerHolder = new ExportSerialzerHolder();

            foreach (var item in PlayerPrefHolderList)
            {
                ExportSerialzer toExport = new ExportSerialzer();
                toExport.type = item.type;
                toExport.key = item.Key;
                toExport.value = item.Value.ToString();
                exportSerialzerHolder.exportlist.Add(toExport);
            }
            string jsonString = JsonUtility.ToJson(exportSerialzerHolder, true);
            return jsonString;
        }
        internal static List<PlayerPrefHolder>  ReadBackupFile(string tempPath)
        {
            string[] filters = new string[] { "text files", "txt", "All files", "*" };
            string path = EditorUtility.OpenFilePanelWithFilters("Load backup file", tempPath, filters);

            if (string.IsNullOrEmpty(path)) return null;



            var stringArray = File.ReadLines(path).Where(line => !line.StartsWith("//")).ToArray();
            var newString = string.Empty;

            foreach (var item in stringArray)
            {
                newString += item;
            }

            ExportSerialzerHolder exportSerialzerHolder = JsonUtility.FromJson<ExportSerialzerHolder>(newString);
            // create pair list from load
            List<PlayerPrefHolder> pairs = new List<PlayerPrefHolder>();

            foreach (var item in exportSerialzerHolder.exportlist)
            {
                PlayerPrefHolder ppp = new PlayerPrefHolder();
                ppp.Key = item.key;
                ppp.TempKey = item.key;
                ppp.type = item.type;

                switch (item.type)
                {
                    case PlayerPrefsType.Int:
                        ppp.Value = int.Parse(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Float:
                        ppp.Value = float.Parse(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.String:
                        ppp.Value = item.value;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Vector2:
                        ppp.Value = AdvancedPlayerPrefs.StringToVector2(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Vector3:
                        ppp.Value = AdvancedPlayerPrefs.StringToVector3(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Vector4:
                        ppp.Value = AdvancedPlayerPrefs.StringToVector4(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;

                        break;
                    case PlayerPrefsType.Color:
                        ppp.Value = AdvancedPlayerPrefs.StringToColor(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.HDRColor:
                        ppp.Value = AdvancedPlayerPrefs.StringToColor(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Bool:
                        ppp.Value = AdvancedPlayerPrefs.StringToBool(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.DateTime:
                        ppp.Value = AdvancedPlayerPrefs.StringToDateTime(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Byte:
                        ppp.Value = AdvancedPlayerPrefs.StringToByte(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Double:
                        ppp.Value = AdvancedPlayerPrefs.StringToDouble(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Vector2Int:
                        ppp.Value = AdvancedPlayerPrefs.StringToVector2Int(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Vector3Int:
                        ppp.Value = AdvancedPlayerPrefs.StringToVector3Int(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    default:
                        break;
                }
                ppp.Save();
                pairs.Add(ppp);
            }
            return pairs;
          //  Refresh();
        }
    }
}

