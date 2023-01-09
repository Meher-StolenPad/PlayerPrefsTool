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
    public class KeysExporter
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
    //[CreateAssetMenu(fileName = "EncryptionSettings", menuName = "EncryptionSettingsHolder/Settings", order = 1)]

    public class EncryptionSettings : ScriptableObject
    {

        //private string Key = "A60A5770FE5E7AB200BA9CFC94E4E8B0";
        //private string Iv = "1234567887654321";
        [SerializeField] private string SavedKey = "A610A2573704FE55E67A7B28008BA79C6FC5944E43E82B01";
        [SerializeField] private string Key="";
        [SerializeField] private string Iv="";

        private char[] Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public string GetKey()
        {
            return Key;
        }
        public string Getiv()
        {
            return Iv;
        }
        private void GetKeysFromSavedKey()
        {
            Key = String.Empty;
            Iv = String.Empty;
            for (int i = 0; i < SavedKey.Length; i+=3)
            {
                Key += SavedKey.Substring(i, 2);
                Iv += SavedKey.Substring(i+3, 1);
            }
        }
        public void SetSavedKeyFromKeys(string key,string iv)
        {
            SavedKey = string.Empty;
            for (int i = 0; i < iv.Length; i++)
            {
                SavedKey += key.Substring(i*2, 2);
                SavedKey += iv[i];
            }
            Debug.Log(SavedKey);
        }
        public void RefreshKeys()
        {
            Key = CreateKey(32);
            Iv = CreateKey(16);
            SetSavedKeyFromKeys(Key, Iv);
        }
        public void ExportKeys()
        {
            Export();
        }
        public void ImportKeys()
        {
            Export();
        }
        public void SetKeys(string _key,string _iv)
        {
            Key = _key;
            Iv = _iv;
        }
        public string CreateKey(int _lenght)
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
                path = PrefsSerialzer.NextAvailableFilename(path);
                File.WriteAllText(path, newBackupString);
            }
            Debug.Log(path);
        }
        private string CreateBackup()
        {
            KeysExporter keysExporter = new KeysExporter(Key, Iv);
            string jsonString = JsonUtility.ToJson(keysExporter, true);
            return jsonString;
        }
    }
}