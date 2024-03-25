using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
#endif
[assembly: InternalsVisibleTo("AdvancedPlayerPrefsEditor")]
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal enum PlayerPrefsType
    {
        Int,
        Float,
        String,
        Byte
    }

    #region Internal Classes Region
    internal class ReturnType
    {
        public PlayerPrefsType PlayerPrefsType = PlayerPrefsType.String;
        public bool IsEncrypted;
    }
    [Serializable]
    internal class Serialzer<T>
    {
        public bool isEncrypted;
        public PlayerPrefsType type;
        [SerializeField]
        public T value;
    }
    [Serializable]
    internal class ExportSerialzer
    {
        public bool isEncrypted;
        public string key;
        public PlayerPrefsType type;
        public string value;
    }
    [Serializable]
    internal class ExportSerialzerHolder
    {
        public List<ExportSerialzer> exportlist = new List<ExportSerialzer>();
    }
    #endregion

    public static class AdvancedPlayerPrefs
    {
        #region Private Variables
        private static readonly string numberPattern = " ({0})";
        private static bool showEncryptionWarning;
        private static bool showDecryptionWarning;
        private static bool? autoEncryption = null;
        internal static bool? AutoEncryption
        {
            get
            {
                if (autoEncryption == null)
                {
                    if (AdvancedPlayerPrefsSettings.Instance != null)
                    {
                        autoEncryption = AdvancedPlayerPrefsSettings.Instance.AutoEncryption;
                        return autoEncryption;
                    }
                    else
                    {
                        autoEncryption = false;
                        return autoEncryption;
                    }
                }
                else
                {
                    if (AdvancedPlayerPrefsSettings.Instance != null)
                    {
                        if (autoEncryption != AdvancedPlayerPrefsSettings.Instance.AutoEncryption)
                        {
                            autoEncryption = AdvancedPlayerPrefsSettings.Instance.AutoEncryption;
                        }
                    }

                    return autoEncryption;
                }
            }
            set => autoEncryption = value;
        }
        internal static Action OnSettingsCreated;

        internal static Action<string, PlayerPrefsType, bool> OnPreferenceUpdated;

        #endregion
        #region Editor Region
#if UNITY_EDITOR
        internal static bool SelectSettings(bool select = true)
        {
            TryLoadSettings();

            string path = AssetDatabase.GetAssetPath(AdvancedPlayerPrefsSettings.Instance);

            if (string.IsNullOrEmpty(path))
            {
                if (!select)
                    DavanciDebug.Warning(AdvancedPlayerPrefsGlobalVariables.NoSettingsWarning);
                return false;
            }
            else
            {
                if (select)
                    Selection.objects = new UnityEngine.Object[] { AdvancedPlayerPrefsSettings.Instance };
                return true;
            }
        }
        internal static void CreateSettings()
        {
            AdvancedPlayerPrefsSettings en = ScriptableObject.CreateInstance<AdvancedPlayerPrefsSettings>();

            if (!Directory.Exists(AdvancedPlayerPrefsGlobalVariables.EncryptionSettingsPath))
            {
                Directory.CreateDirectory(AdvancedPlayerPrefsGlobalVariables.EncryptionSettingsPath);
            }
            AssetDatabase.CreateAsset(en, AdvancedPlayerPrefsGlobalVariables.EncryptionSettingsPath + AdvancedPlayerPrefsGlobalVariables.EncryptionSettingsFileName);
            AdvancedPlayerPrefsSettings.Reload();
            OnSettingsCreated?.Invoke();
        }
#endif
        #endregion

        #region Initialization Region

        private static bool TryLoadSettings()
        {
            if (AdvancedPlayerPrefsSettings.Instance == null) return false;
            return true;
        }
        #endregion

        #region Legacy PlayerPrefs Functions
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
        #endregion

        #region Get Variable Region
        /// <summary>
        /// Gets the integer value for the given key in PlayerPrefs
        /// </summary>
        /// <param name="key">The key of the preference.</param>
        /// <param name="defaultValue">The default value to return if the preference doesn't exist.</param>
        /// <returns>The value of the preference,or the default value if the key is not found in PlayerPrefs.</returns>
        public static int GetInt(string key, int defaultValue = int.MinValue)
        {
            int returnInt = PlayerPrefs.GetInt(key, defaultValue);

            if (returnInt == defaultValue)
            {
                returnInt = GetCosutomTypeValue<int>(key, defaultValue);
            }
            return returnInt;
        }
        /// <summary>
        /// Gets the Float value for the given key in PlayerPrefs
        /// </summary>
        /// <param name="key">The key of the preference.</param>
        /// <param name="defaultValue">The default value to return if the preference doesn't exist.</param>
        /// <returns>The value of the preference,or the default value if the key is not found in PlayerPrefs.</returns>
        public static float GetFloat(string key, float defaultValue = float.MinValue)
        {
            float returnFloat = PlayerPrefs.GetFloat(key, defaultValue);

            if (returnFloat == defaultValue)
            {
                returnFloat = GetCosutomTypeValue<float>(key, defaultValue);
            }

            return returnFloat;
        }
        /// <summary>
        /// Gets the string value for the given key in PlayerPrefs
        /// </summary>
        /// <param name="key">The key of the preference.</param>
        /// <param name="defaultValue">The default value to return if the preference doesn't exist.</param>
        /// <returns>The value of the preference,or the default value if the key is not found in PlayerPrefs.</returns>
        public static string GetString(string key, string defaultValue = "")
        {
            return DecryptionAES(PlayerPrefs.GetString(key, defaultValue));
        }


        /// <summary>
        /// Gets the byte value for the given key in PlayerPrefs
        /// </summary>
        /// <param name="key">The key of the preference.</param>
        /// <param name="defaultValue">The default value to return if the preference doesn't exist.</param>
        /// <returns>The value of the preference,or the default value if the key is not found in PlayerPrefs.</returns>
        public static byte GetByte(string key, byte defaultValue = byte.MinValue)
        {
            return GetCosutomTypeValue<byte>(key, defaultValue);
        }
        #endregion

        #region Set Variable Region
        /// <summary>
        /// Sets an integer value for the given key in PlayerPrefs with an optional encryption option.
        /// </summary>
        /// <param name="key">The key of the preference.</param>
        /// <param name="value">The value of the preference.</param>
        /// <param name="useEncryption">Whether or not to encrypt the preference value using AES encryption.<br/>
        /// Note: If AutoEncryption is enabled in the settings, you can ignore this parameter.</param>
        public static void SetInt(string key, int value, bool useEncryption = false)
        {
            if (AutoEncryption != null) useEncryption = (bool)AutoEncryption || useEncryption;

            if (useEncryption)
            {
                Serialzer<int> serialzer = new Serialzer<int>
                {
                    type = PlayerPrefsType.Int,
                    value = value,
                    isEncrypted = useEncryption
                };
                string jsonString = JsonUtility.ToJson(serialzer);

                if (TryEncryptionAES(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                }
                else
                {
                    PlayerPrefs.SetInt(key, value);
                }
            }
            else
            {
                PlayerPrefs.SetInt(key, value);
            }
            DavanciDebug.Log("Set Int : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
            OnPreferenceUpdated?.Invoke(key, PlayerPrefsType.Int, useEncryption);
        }
        /// <summary>
        /// Sets a float value for the given key in PlayerPrefs with an optional encryption option.
        /// </summary>
        /// <param name="key">The key of the preference.</param>
        /// <param name="value">The value of the preference.</param>
        /// <param name="useEncryption">Whether or not to encrypt the preference value using AES encryption.<br/>
        /// Note: If AutoEncryption is enabled in the settings, you can ignore this parameter.</param>
        public static void SetFloat(string key, float value, bool useEncryption = false)
        {
            if (AutoEncryption != null)
            {
                useEncryption = (bool)autoEncryption;
            }
            if (useEncryption)
            {
                Serialzer<float> serialzer = new Serialzer<float>
                {
                    type = PlayerPrefsType.Float,
                    isEncrypted = useEncryption,

                    value = value
                };
                string jsonString = JsonUtility.ToJson(serialzer);

                if (TryEncryptionAES(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                }
                else
                {
                    PlayerPrefs.SetFloat(key, value);
                }
            }
            else
            {
                PlayerPrefs.SetFloat(key, value);
            }
            OnPreferenceUpdated?.Invoke(key, PlayerPrefsType.Float, useEncryption);
            DavanciDebug.Log("Set Float : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        /// <summary>
        /// Sets a string value for the given key in PlayerPrefs with an optional encryption option.
        /// </summary>
        /// <param name="key">The key of the preference.</param>
        /// <param name="value">The value of the preference.</param>
        /// <param name="useEncryption">Whether or not to encrypt the preference value using AES encryption.<br/>
        /// Note: If AutoEncryption is enabled in the settings, you can ignore this parameter.</param>
        public static void SetString(string key, string value, bool useEncryption = false)
        {
            if (AutoEncryption != null) useEncryption = (bool)AutoEncryption || useEncryption;

            if (useEncryption)
            {
                Serialzer<string> serialzer = new Serialzer<string>
                {
                    type = PlayerPrefsType.String,
                    value = value,
                    isEncrypted = useEncryption
                };

                string jsonString = JsonUtility.ToJson(serialzer);

                if (TryEncryptionAES(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                }
                else
                {
                    PlayerPrefs.SetString(key, value);
                }
            }
            else
            {
                PlayerPrefs.SetString(key, value);
            }
            DavanciDebug.Log("Set String : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
            OnPreferenceUpdated?.Invoke(key, PlayerPrefsType.String, useEncryption);
        }

        /// <summary>
        /// Sets a Byte value for the given key in PlayerPrefs with an optional encryption option.
        /// </summary>
        /// <param name="key">The key of the preference.</param>
        /// <param name="_value">The value of the preference.</param>
        /// <param name="useEncryption">Whether or not to encrypt the preference value using AES encryption.<br/>
        /// Note: If AutoEncryption is enabled in the settings, you can ignore this parameter.</param>
        public static void SetByte(string key, byte _value, bool useEncryption = false)
        {
            if (AutoEncryption != null) useEncryption = (bool)AutoEncryption || useEncryption;

            Serialzer<byte> serialzer = new Serialzer<byte>
            {
                type = PlayerPrefsType.Byte,
                value = _value,
                isEncrypted = useEncryption
            };

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryptionAES(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                }
                else
                {
                    PlayerPrefs.SetString(key, jsonString);
                }
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
            OnPreferenceUpdated?.Invoke(key, PlayerPrefsType.Byte, useEncryption);
            DavanciDebug.Log("Set Byte : " + key + ", Use Encryption : " + useEncryption, Color.cyan);

        }
        #endregion

        #region Json Data Region / Internal data 
        private static T GetCosutomTypeValue<T>(string key, T defaultValue = default)
        {
            string savedValue = PlayerPrefs.GetString(key);

            string d = DecryptionAES(savedValue);
            object returnvalue;
            if (d.TryParseJson(out Serialzer<T> t))
            {
                Serialzer<T> serialzer = t;
                returnvalue = serialzer.value;
            }
            else
            {
                returnvalue = defaultValue;

            }
            return (T)returnvalue;
        }
        internal static string ReturnObjectValue(Serialzer<object> data)
        {
            return data.value.ToString();
        }
        internal static object TryGetCostumeType(string key, out ReturnType returnType, string defaultValue = "")
        {
            string json = PlayerPrefs.GetString(key, defaultValue);
            returnType = new ReturnType();


            object retunValue;
            //Debug.Log(json);

            if (String.IsNullOrEmpty(json))
            {
                returnType.PlayerPrefsType = PlayerPrefsType.String;
                retunValue = json;
                Debug.Log(key + " Is empty !");
            }
            else if (json.TryParseJson(out Serialzer<object> t))
            {
                returnType.PlayerPrefsType = t.type;

                retunValue = ReturnObjectValue(t);
            }
            else
            {
                returnType.PlayerPrefsType = PlayerPrefsType.String;

                // test if encrypted data

                string decryptedString = DecryptionAES(json);

                if (decryptedString.TryParseJson(out Serialzer<object> data))
                {
                    returnType.PlayerPrefsType = data.type;
                    retunValue = data.value;
                    returnType.IsEncrypted = true;
                }
                else
                {
                    retunValue = json;
                }
            }
            return retunValue;
        }
        private static bool TryParseJson<T>(this string @this, out T result)
        {
            result = default;

            if (string.IsNullOrEmpty(@this)) return false;
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };

            result = JsonConvert.DeserializeObject<T>(@this, settings);
            return success;
        }
        #endregion

        #region string to Variable Region
        internal static int StringToInt(string s)
        {
            return int.Parse(s);
        }
        internal static void SetFloat(string key, string value, bool useEncryption = false)
        {
            var floatValue = float.Parse(value);
            if (useEncryption)
            {
                Serialzer<float> serialzer = new Serialzer<float>
                {
                    type = PlayerPrefsType.Float,
                    value = floatValue,
                    isEncrypted = useEncryption
                };

                string jsonString = JsonUtility.ToJson(serialzer);

                if (TryEncryptionAES(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                }
                else
                {
                    PlayerPrefs.SetFloat(key, floatValue);
                }
            }
            else
            {
                PlayerPrefs.SetFloat(key, floatValue);
            }
        }
        public static bool ApproximatelyEqualEpsilon(float a, float b, float epsilon)
        {
            const float floatNormal = (1 << 23) * float.Epsilon;
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b)
            {
                // Shortcut, handles infinities
                return true;
            }

            if (a == 0.0f || b == 0.0f || diff < floatNormal)
            {
                // a or b is zero, or both are extremely close to it.
                // relative error is less meaningful here
                return diff < (epsilon * floatNormal);
            }

            // use relative error
            return diff / Math.Min((absA + absB), float.MaxValue) < epsilon;
        }
        internal static float StringToFloat(string s)
        {
            return float.Parse(s);
        }
        internal static byte StringToByte(string s)
        {
            return Byte.Parse(s);
        }
        #endregion

        #region File Exporter 
        internal static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }
        internal static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2;

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        #endregion

        #region Encryption Region
        internal static bool TryEncryptionAES(string inputData, out string _result)
        {
            if (AdvancedPlayerPrefsSettings.Instance == null)
            {
                if (!showEncryptionWarning)
                {
                    DavanciDebug.Warning(AdvancedPlayerPrefsGlobalVariables.NoEncryptionSettingsWarning);
                    showEncryptionWarning = true;
                }
                _result = inputData;
                return false;
            }

            AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider
            {
                BlockSize = 128,
                KeySize = 256,
                Key = ASCIIEncoding.ASCII.GetBytes(AdvancedPlayerPrefsSettings.Instance.GetKey()),
                IV = ASCIIEncoding.ASCII.GetBytes(AdvancedPlayerPrefsSettings.Instance.Getiv()),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            byte[] txtByteData = ASCIIEncoding.ASCII.GetBytes(inputData);
            ICryptoTransform trnsfrm = AEScryptoProvider.CreateEncryptor(AEScryptoProvider.Key, AEScryptoProvider.IV);

            byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
            _result = Convert.ToBase64String(result);
            return true;
        }
        internal static string DecryptionAES(string inputData)
        {
            string returnstring = inputData;
            if (AdvancedPlayerPrefsSettings.Instance == null)
            {
                if (!showDecryptionWarning)
                {
                    DavanciDebug.Warning(AdvancedPlayerPrefsGlobalVariables.NoDecryptionSettingsWarning);
                    showDecryptionWarning = true;
                }
                return returnstring;
            }
            try
            {
                AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider
                {
                    BlockSize = 128,
                    KeySize = 256,
                    Key = ASCIIEncoding.ASCII.GetBytes(AdvancedPlayerPrefsSettings.Instance.GetKey()),
                    IV = ASCIIEncoding.ASCII.GetBytes(AdvancedPlayerPrefsSettings.Instance.Getiv()),
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };

                try
                {
                    byte[] txtByteData = Convert.FromBase64String(inputData);
                    ICryptoTransform trnsfrm = AEScryptoProvider.CreateDecryptor();

                    byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
                    returnstring = ASCIIEncoding.ASCII.GetString(result);
                }
                catch (Exception e)
                {
                    string ex = e.ToString();
                }
            }
            catch (Exception e)
            {
                _ = e.ToString();
            }

            return returnstring;
        }
        #endregion

    }
}