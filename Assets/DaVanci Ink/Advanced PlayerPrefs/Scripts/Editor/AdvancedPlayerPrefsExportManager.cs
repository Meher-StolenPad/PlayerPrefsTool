using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal enum SavePathType : int
    {
        Project = 0,
        Assets = 1,
        Persistant = 2,
        Temporary = 3,
        Absolute = 4,
        None = 5
    }
    internal static class AdvancedPlayerPrefsExportManager
    {
        internal static SavePathType m_savePathType;
        internal static string m_exportPath;

        internal static string GetPathByType(SavePathType m_savePathType = SavePathType.Project)
        {
            string path = string.Empty;
            switch (m_savePathType)
            {
                case SavePathType.Project:
                    path = Application.dataPath.Replace("/Assets", "");
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
                m_exportPath = GetPathByType(_savePathType);

            var backupstring = CreateBackup(PlayerPrefHolderList);
            string newBackupString = AdvancedPlayerPrefsGlobalVariables.BackupCreatedText;
            string playerprefsSpecific = "//Player prefs for product  : " + Application.productName + " , Company :  " + Application.companyName + '\n'
                + "//Created at : " + DateTime.Now + "\n//Created by " + UnityEditor.CloudProjectSettings.userName + '\n';
            newBackupString += playerprefsSpecific;

            newBackupString += backupstring;

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
            DavanciDebug.Log("File Exported : " + ExportPathFile, Color.green);
        }
        private static string CreateBackup(List<PlayerPrefHolder> PlayerPrefHolderList)
        {
            ExportSerialzerHolder exportSerialzerHolder = new ExportSerialzerHolder();

            foreach (var item in PlayerPrefHolderList)
            {
                ExportSerialzer toExport = new ExportSerialzer
                {
                    type = item.type,
                    key = item.Key,
                    value = item.Value.ToString(),
                    isEncrypted = item.isEncrypted
                };
                exportSerialzerHolder.exportlist.Add(toExport);
            }
            string jsonString = JsonUtility.ToJson(exportSerialzerHolder, true);
            return jsonString;
        }
        internal static List<PlayerPrefHolder> ReadBackupFile(string tempPath)
        {
            string[] filters = AdvancedPlayerPrefsGlobalVariables.OpenFolderFilters;

            string path = EditorUtility.OpenFilePanelWithFilters("Load backup file", tempPath, filters);

            if (string.IsNullOrEmpty(path)) return null;

            var stringArray = File.ReadLines(path).Where(line => !line.StartsWith("//")).ToArray();
            var newString = string.Empty;

            foreach (var item in stringArray)
            {
                newString += item;
            }

            ExportSerialzerHolder exportSerialzerHolder = JsonUtility.FromJson<ExportSerialzerHolder>(newString);

            List<PlayerPrefHolder> pairs = new List<PlayerPrefHolder>();

            if (exportSerialzerHolder == null) return pairs;
            // create pair list from load

            foreach (var item in exportSerialzerHolder.exportlist)
            {
                PlayerPrefHolder ppp = ScriptableObject.CreateInstance<PlayerPrefHolder>();
                ppp.Key = item.key;
                ppp.TempKey = item.key;
                ppp.type = item.type;
                ppp.isEncrypted = item.isEncrypted;
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
                    case PlayerPrefsType.Long:
                        ppp.Value = AdvancedPlayerPrefs.StringToLong(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayInt:
                        ppp.arrayInt = AdvancedPlayerPrefs.StringToArrayInt(item.value);
                        ppp.Value = ppp.arrayInt;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayFloat:
                        ppp.arrayFloat = AdvancedPlayerPrefs.StringToArrayFloat(item.value);
                        ppp.Value = ppp.arrayFloat;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayString:
                        ppp.arrayString = AdvancedPlayerPrefs.StringToArrayString(item.value);
                        ppp.Value = ppp.arrayString;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayDouble:
                        ppp.arrayDouble = AdvancedPlayerPrefs.StringToArrayDouble(item.value);
                        ppp.Value = ppp.arrayDouble;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayLong:
                        ppp.arrayLong = AdvancedPlayerPrefs.StringToArrayLong(item.value);
                        ppp.Value = ppp.arrayLong;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayBool:
                        ppp.arrayBool = AdvancedPlayerPrefs.StringToArrayBool(item.value);
                        ppp.Value = ppp.arrayBool;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayByte:
                        ppp.arrayByte = AdvancedPlayerPrefs.StringToArrayByte(item.value);
                        ppp.Value = ppp.arrayByte;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayVector3:
                        ppp.arrayVector3 = AdvancedPlayerPrefs.StringToArrayVector3(item.value);
                        ppp.Value = ppp.arrayVector3;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayVector3Int:
                        ppp.arrayVector3Int = AdvancedPlayerPrefs.StringToArrayVector3Int(item.value);
                        ppp.Value = ppp.arrayVector3Int;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayVector2:
                        ppp.arrayVector2 = AdvancedPlayerPrefs.StringToArrayVector2(item.value);
                        ppp.Value = ppp.arrayVector2;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayVector2Int:
                        ppp.arrayVector2Int = AdvancedPlayerPrefs.StringToArrayVector2Int(item.value);
                        ppp.Value = ppp.arrayVector2Int;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.ArrayVector4:
                        ppp.arrayVector4 = AdvancedPlayerPrefs.StringToArrayVector4(item.value);
                        ppp.Value = ppp.arrayVector4;
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    default:
                        break;
                }
                ppp.Save();
                pairs.Add(ppp);
            }
            DavanciDebug.Log(pairs.Count + " Prefs Imported from < " + Path.GetFileName(path) + ">", Color.green);
            return pairs;
            //  Refresh();
        }
    }
}

