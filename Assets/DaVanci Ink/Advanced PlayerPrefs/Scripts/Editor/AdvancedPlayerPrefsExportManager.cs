using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
                ExportPathFile =AdvancedPlayerPrefs.NextAvailableFilename(ExportPathFile);
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
        internal static List<PlayerPrefHolder>  ReadBackupFile(string tempPath)
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
                PlayerPrefHolder ppp =ScriptableObject.CreateInstance <PlayerPrefHolder>();
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
                }
                ppp.Save();
                pairs.Add(ppp);
            }
            DavanciDebug.Log(pairs.Count+" Prefs Imported from < " + Path.GetFileName(path)+">", Color.green);
            return pairs;
            //  Refresh();
        }
    }
}

