using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    [Serializable]
    internal class KeysExporter
    {
        public string Key;

        public KeysExporter()
        {
        }

        public KeysExporter(string key)
        {
            Key = key;
        }
    }
    //[CreateAssetMenu(fileName = "EncryptionSettings", menuName = "EncryptionSettingsHolder/Settings", order = 1)]

    internal class EncryptionSettings : ScriptableObject
    {

        public string Key = "A60A5770FE5E7AB200BA9CFC94E4E8B0";
        public readonly string Iv = "1234567887654321";
        public string SavedKey = "A610A2573704FE55E67A7B28008BA79C6FC5944E43E82B01";
        private char[] Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        [HideInInspector] internal bool useDeviceKey;
        internal string GetKey()
        {
            return Key;
        }
        internal string Getiv()
        {
            return Iv;
        }
        private void GetKeysFromSavedKey()
        {
            Key = String.Empty;
            for (int i = 0; i < SavedKey.Length; i+=3)
            {
                Key += SavedKey.Substring(i, 2);
            }
        }
        internal void SetSavedKeyFromKeys()
        {
            SavedKey = string.Empty;
            for (int i = 0; i < Iv.Length; i++)
            {
                SavedKey += Key.Substring(i*2, 2);
                SavedKey += Iv[i];
            }
        }
        internal void RefreshKeys()
        {
            Key = CreateKey(32);
            SetSavedKeyFromKeys();
        }
        internal void ExportKeys()
        {
            Export();
        }
        internal void ImportKeys()
        {
            Export();
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
        private void Export()
        {
            var backupstring = CreateBackup();
            string newBackupString = "//File Created with Player prefs editor Tool\n";
            string playerprefsSpecific = "//Keys for product  : " + Application.productName + " , Company :  " + Application.companyName + '\n'
                + "//Created at : " + DateTime.Now + "\n//Created by " + UnityEditor.CloudProjectSettings.userName + '\n';
            newBackupString += playerprefsSpecific;

            newBackupString += backupstring;

            string path = EditorUtility.OpenFolderPanel("Backup Keys path", "", "PPKeysBackup.txt");
            path += "/PPKeysBackup.txt";

            if (!File.Exists(path))
            {
                File.WriteAllText(path, newBackupString);
            }
            else
            {
                path = AdvancedPlayerPrefs.NextAvailableFilename(path);
                File.WriteAllText(path, newBackupString);
            }
            Debug.Log(path);
        }
        private string CreateBackup()
        {
            KeysExporter keysExporter = new KeysExporter(Key);
            string jsonString = JsonUtility.ToJson(keysExporter, true);
            return jsonString;
        }
    }
}