using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal enum DebugMode
    {
        SilentMode,
        EditorOnly,
        Actif
    }

    [Serializable]
    internal class KeysExporter
    {
        public string Key;
        public string Iv;

        public KeysExporter()
        {
        }

        public KeysExporter(string key, string iv)
        {
            Key = key;
            Iv = iv;
        }
    }

    internal class AdvancedPlayerPrefsSettings : DavanciInkSingleton<AdvancedPlayerPrefsSettings>
    {
        private readonly char[] Chars = AdvancedPlayerPrefsGlobalVariables.CharsKey.ToCharArray();

        public string Key = AdvancedPlayerPrefsGlobalVariables.InitialKey;
        public string Iv = AdvancedPlayerPrefsGlobalVariables.InitialIv;

        private string OldKey = AdvancedPlayerPrefsGlobalVariables.InitialKey;
        private string OldIv = AdvancedPlayerPrefsGlobalVariables.InitialIv;

        [HideInInspector] public DebugMode debugMode = DebugMode.EditorOnly;

        internal string GetKey()
        {
            return Key;
        }
        internal string Getiv()
        {
            return Iv;
        }
      
       
        internal void RefreshKeys()
        {
            Key = CreateKey(32);
            Iv = CreateKey(16);
            DavanciDebug.Log("Keys Refreshed !", Color.cyan);
        }
        internal void SetKeys(string _key)
        {
            Key = _key;
        }
        internal string CreateKey(int _lenght)
        {
            byte[] data = new byte[_lenght];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(_lenght);
            foreach (byte b in data)
            {
                result.Append(Chars[b % (Chars.Length)]);
            }
            return result.ToString();
        }
#if UNITY_EDITOR
        public void SaveOldKeys()
        {
            OldKey = Key;
            OldIv = Iv;
        }
        public void GetOldKeys()
        {
            Key = OldKey;
            Iv = OldIv;
        }

        internal void ExportKeys()
        {
            Export();
        }
        private void Export()
        {
            var backupstring = AdvancedPlayerPrefsGlobalVariables.GetPlayerPrefsSpecificText() + CreateBackup();

            string path = EditorUtility.OpenFolderPanel(AdvancedPlayerPrefsGlobalVariables.KeyOpenPanelTitle, "", AdvancedPlayerPrefsGlobalVariables.KeyBackupFileName);
            path += "/" + AdvancedPlayerPrefsGlobalVariables.KeyBackupFileName + ".txt";

            if (!File.Exists(path))
            {
                File.WriteAllText(path, backupstring);
            }
            else
            {
                path = AdvancedPlayerPrefs.NextAvailableFilename(path);
                File.WriteAllText(path, backupstring);
            }
            DavanciDebug.Log("Keys Exported : " + path, Color.green);
        }
        private string CreateBackup()
        {
            KeysExporter keysExporter = new KeysExporter(Key, Iv);
            string jsonString = JsonUtility.ToJson(keysExporter, true);
            return jsonString;
        }
        internal string ReadBackupFile()
        {
            try
            {
                string path = EditorUtility.OpenFilePanelWithFilters("Load keys backup file", "", AdvancedPlayerPrefsGlobalVariables.OpenFolderFilters);

                if (string.IsNullOrEmpty(path)) return string.Empty;

                var stringArray = File.ReadLines(path).Where(line => !line.StartsWith("//")).ToArray();
                var newString = string.Empty;

                foreach (var item in stringArray)
                {
                    newString += item;
                }

                KeysExporter KeysExporter = JsonUtility.FromJson<KeysExporter>(newString);
                DavanciDebug.Log("Keys Imported from < " + Path.GetFileName(path) + " >", Color.green);
                return KeysExporter.Key;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                DavanciDebug.Error(ex.ToString());
                return string.Empty;
            }


        }
#endif
    }
}