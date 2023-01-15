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
            Debug.Log("CheckKey");
            if (useDeviceKey)
            {
                if (AdvancedPlayerPrefs.HasAPPsCSDK())
                {
                    //load old saved key
                    SavedKey = AdvancedPlayerPrefs.GetAPPsCSDK();
                    GetKeysFromSavedKey();
                }
                else
                {
                    // create new key and save it
                    RefreshKeys();
                    SaveKey();
                }
            }
        }

#if UNITY_EDITOR
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
            Debug.Log(path);
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
                return KeysExporter.Key;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return  string.Empty;
            }

             
        }
#endif
    }
}