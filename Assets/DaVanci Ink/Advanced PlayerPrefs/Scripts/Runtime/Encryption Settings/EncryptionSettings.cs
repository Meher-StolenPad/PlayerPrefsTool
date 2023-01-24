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
    [Serializable]
    internal class KeysExporter
    {
        public string Key;
        public string Iv;

        public KeysExporter()
        {
        }

        public KeysExporter(string key,string iv)
        {
            Key = key;
            Iv = iv;
        }
    }

    internal class EncryptionSettings : ScriptableObject
    {
        private char[] Chars = AdvancedPlayerPrefsGlobalVariables.CharsKey.ToCharArray();

        public string Key = AdvancedPlayerPrefsGlobalVariables.InitialKey;
        public string Iv = AdvancedPlayerPrefsGlobalVariables.InitialIv;
        public string SavedKey = AdvancedPlayerPrefsGlobalVariables.InitialSavedKey;

        private string OldKey = AdvancedPlayerPrefsGlobalVariables.InitialKey;
        private string OldIv = AdvancedPlayerPrefsGlobalVariables.InitialIv;
        private string OldSavedKey = AdvancedPlayerPrefsGlobalVariables.InitialSavedKey;

        [HideInInspector] public bool useDeviceKey;
        internal string GetKey()
        {
            return Key;
        }
        internal string Getiv()
        {
            return Iv;
        }
        internal void GetKeysFromSavedKey()
        {
            Key = String.Empty;
            Iv = String.Empty;
            for (int i = 0; i < SavedKey.Length; i += 4)
            {
                Key += SavedKey.Substring(i, 2);
                Iv += SavedKey.Substring(i + 3, 1);
            }
        }
        internal void SaveKey()
        {
            SetSavedKeyFromKeys();
            AdvancedPlayerPrefs.SetAPPsCSDK(SavedKey);
        }
        internal void SetSavedKeyFromKeys()
        {
            SavedKey = string.Empty;
            string cryptoText=  CreateKey(16);

            for (int i = 0; i < Iv.Length; i++)
            {
                SavedKey += Key.Substring(i * 2, 2);
                SavedKey += cryptoText[i];
                SavedKey += Iv[i];
            }
        }
        internal void RefreshKeys()
        {
            Key = CreateKey(32);
            Iv = CreateKey(16);
            SetSavedKeyFromKeys();
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

        internal void CheckKey()
        {
            if (useDeviceKey)
            {
                if (AdvancedPlayerPrefs.HasAPPsCSDK())
                {
                    //load old saved key
                    DavanciDebug.Log("Load Device Key!", Color.grey);
                    SavedKey = AdvancedPlayerPrefs.GetAPPsCSDK();
                    GetKeysFromSavedKey();
                }
                else
                {
                    DavanciDebug.Log("Device Key Created !", Color.grey);
                    // create new key and save it
                    RefreshKeys();
                    SaveKey();
                }
            }
        }

#if UNITY_EDITOR
        public void SaveOldKeys()
        {
            OldKey = Key;
            OldIv = Iv;
            OldSavedKey = SavedKey;
        }
        public void GetOldKeys()
        {
            Key = OldKey;
            Iv = OldIv;
            SavedKey = OldSavedKey;
        }

        internal void ExportKeys()
        {
            Export();
        }
        private void Export()
        {
            var backupstring = AdvancedPlayerPrefsGlobalVariables.GetPlayerPrefsSpecificText()+  CreateBackup();
           
            string path = EditorUtility.OpenFolderPanel(AdvancedPlayerPrefsGlobalVariables.KeyOpenPanelTitle, "", AdvancedPlayerPrefsGlobalVariables.KeyBackupFileName);
            path += "/"+AdvancedPlayerPrefsGlobalVariables.KeyBackupFileName+".txt";

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
            KeysExporter keysExporter = new KeysExporter(Key,Iv);
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
                DavanciDebug.Log("Keys Imported from < " +Path.GetFileName(path)+" >", Color.green);
                return KeysExporter.Key;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                DavanciDebug.Error(ex.ToString());
                return  string.Empty;
            }

             
        }
#endif
    }
}