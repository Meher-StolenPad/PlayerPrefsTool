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
        public string Lv;

        public KeysExporter()
        {
        }

        public KeysExporter(string key, string lv)
        {
            Key = key;
            Lv = lv;
        }
    }
    [CreateAssetMenu(fileName = "EncryptionSettings", menuName = "EncryptionSettingsHolder/Settings", order = 1)]

    public class EncryptionSettings : ScriptableObject
    {
        public string Key = "A60A5770FE5E7AB200BA9CFC94E4E8B0";
        public string Lv = "1234567887654321";

        private char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        public void RefreshKeys()
        {
            Key = CreateKey(32);
            Lv = CreateKey(16);
        }
        public void ExportKeys()
        {
            Export();
        }
        public void ImportKeys()
        {
            Export();
        }
        public void SetKeys(string _key,string _lv)
        {
            Key = _key;
            Lv = _lv;
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
                result.Append(chars[b % (chars.Length)]);
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
            KeysExporter keysExporter = new KeysExporter(Key, Lv);
            string jsonString = JsonUtility.ToJson(keysExporter, true);
            return jsonString;
        }
    }
}