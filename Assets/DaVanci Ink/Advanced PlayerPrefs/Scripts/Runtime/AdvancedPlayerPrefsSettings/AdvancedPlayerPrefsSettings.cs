using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;
using System.Linq;
using Random = System.Random;
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
    internal enum BackupMode
    {
        Auto_Update,
        Manual_Update,
        Disable
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
        public string SavedKey = AdvancedPlayerPrefsGlobalVariables.InitialSavedKey;
#if UNITY_EDITOR
        private string OldKey = AdvancedPlayerPrefsGlobalVariables.InitialKey;
        private string OldIv = AdvancedPlayerPrefsGlobalVariables.InitialIv;
        private string OldSavedKey = AdvancedPlayerPrefsGlobalVariables.InitialSavedKey;
#endif
        [HideInInspector] public bool AutoEncryption;

        [HideInInspector] public DebugMode debugMode = DebugMode.EditorOnly;
        [HideInInspector] public BackupMode backupMode = BackupMode.Manual_Update;
        internal static Action OnBackupModeChangedAction;

        internal void OnBackupModeChanged()
        {
            Debug.Log("OnBackupModeChanged" + backupMode);
            OnBackupModeChangedAction?.Invoke();
        }
        internal string GetKey()
        {
            return Key;
        }
        internal string Getiv()
        {
            return Iv;
        }
        internal void SaveKey()
        {
            SetSavedKeyFromKeys();
        }
        internal void GetKeysFromSavedKey()
        {
            Key = "";
            Iv = "";
            for (int i = 0; i < SavedKey.Length; i += 4)
            {
                if (i + 1 < SavedKey.Length)
                {
                    Key += SavedKey[i];
                    Key += SavedKey[i + 1];
                }

                if (i + 3 < SavedKey.Length)
                {
                    Iv += SavedKey[i + 3];
                }
            }
        }
        internal void SetSavedKeyFromKeys()
        {
            string randomString = CreateRandomString(32);

            // Create a string builder to store the merged string
            StringBuilder mergedString = new StringBuilder();

            // Merge the key, random string, and IV into the string builder
            int keyIndex = 0;
            int ivIndex = 0;
            int randomIndex = 0;
            while (keyIndex < Key.Length || ivIndex < Iv.Length)
            {
                if (keyIndex < Key.Length)
                {
                    mergedString.Append(Key[keyIndex++]);
                    mergedString.Append(Key[keyIndex++]);
                }

                if (randomIndex < 32)
                {
                    mergedString.Append(randomString[randomIndex++]);
                }

                if (ivIndex < Iv.Length)
                {
                    mergedString.Append(Iv[ivIndex++]);
                }
            }

            // Convert the string builder to a string
            SavedKey = mergedString.ToString();
        }
        private static string CreateRandomString(int length)
        {
            // Create a random number generator
            Random rand = new Random();

            // Create a string builder to store the random string
            StringBuilder randomString = new StringBuilder();

            // Generate the random string
            for (int i = 0; i < length; i++)
            {
                char randomChar = (char)(rand.Next(33, 126));
                randomString.Append(randomChar);
            }

            return randomString.ToString();
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