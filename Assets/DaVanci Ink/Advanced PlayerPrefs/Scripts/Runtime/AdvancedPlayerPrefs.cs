using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Byte,
        Bool,
        Double,
        Vector2,
        Vector2Int,
        Vector3,
        Vector3Int,
        Vector4,
        Color,
        HDRColor,
        DateTime
    }

    internal class ReturnType
    {
        public PlayerPrefsType PlayerPrefsType = PlayerPrefsType.String;
        public bool IsEncrypted;
    }
    public static class AdvancedPlayerPrefs
    {
        private static readonly string EncryptionSettingsPath = "Assets/Resources/AdvancedPlayerPrefs/";
        private static readonly string EncryptionSettingsResourcesPath = "AdvancedPlayerPrefs/EncryptionSettings";
        private static readonly string EncryptionSettingsFileName = "EncryptionSettings.asset";

        private static string numberPattern = " ({0})";
        private static EncryptionSettings EncryptionSettings = null;
        private static bool isInitialzed;

#if UNITY_EDITOR
        internal static bool SelectSettings(bool select = true)
        {
            TryLoadSettings();

            string path = AssetDatabase.GetAssetPath(EncryptionSettings);

            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            else
            {
                if (select)
                    Selection.objects = new UnityEngine.Object[] { EncryptionSettings };
                return true;
            }
        }
        internal static void CreateSettings()
        {
            EncryptionSettings en = ScriptableObject.CreateInstance<EncryptionSettings>();

            if (!Directory.Exists(EncryptionSettingsPath))
            {
                Directory.CreateDirectory(EncryptionSettingsPath);
            }
            AssetDatabase.CreateAsset(en, EncryptionSettingsPath + EncryptionSettingsFileName);
            EncryptionSettings = en;
        }
#endif
        private static void Init()
        {
            if (isInitialzed) return;

            if (!EncryptionSettings)
            {
                if (TryLoadSettings())
                {
                    EncryptionSettings.CheckKey();
                }
            }
            isInitialzed = true;
        }
        private static bool TryLoadSettings()
        {
            if (EncryptionSettings == null)
            {
                EncryptionSettings = Resources.Load<EncryptionSettings>(EncryptionSettingsResourcesPath);
                if (EncryptionSettings == null)
                {
                    Debug.LogWarning(AdvancedPlayerPrefsGlobalVariables.NoEncryptionSettingsWarning);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
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
        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        public static float GetFloat(string key, float defaultValue = float.MinValue)
        {
            float returnFloat = PlayerPrefs.GetFloat(key, defaultValue);

            if (returnFloat == defaultValue)
            {
                returnFloat = GetCosutomTypeValue<float>(key, defaultValue);
            }

            return returnFloat;
        }
        public static string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        public static Vector2 GetVector2(string key, Vector2 defaultValue)
        {
            return GetCosutomTypeValue<Vector2>(key, defaultValue);
        }
        public static Vector3 GetVector3(string key, Vector3 defaultValue)
        {
            return GetCosutomTypeValue<Vector3>(key, defaultValue);
        }
        public static Vector4 GetVector4(string key, Vector4 defaultValue)
        {
            return GetCosutomTypeValue<Vector4>(key, defaultValue);
        }
        public static Color GetColor(string key, Color defaultValue, bool hdr)
        {
            return GetCosutomTypeValue<Color>(key, defaultValue);
        }
        public static bool GetBool(string key, bool defaultValue)
        {
            return GetCosutomTypeValue<bool>(key, defaultValue);
        }
        public static byte GetByte(string key, byte defaultValue)
        {
            return GetCosutomTypeValue<byte>(key, defaultValue);
        }
        public static double GetDouble(string key, double defaultValue)
        {
            return GetCosutomTypeValue<double>(key, defaultValue);
        }
        public static Vector2Int GetVector2Int(string key, Vector2Int defaultValue)
        {
            return GetCosutomTypeValue<Vector2Int>(key, defaultValue);
        }
        public static Vector3Int GetVector3Int(string key, Vector3Int defaultValue)
        {
            return GetCosutomTypeValue<Vector3Int>(key, defaultValue);
        }
        private static T GetCosutomTypeValue<T>(string key, T defaultValue)
        {
            object returnvalue = default;

            string savedValue = PlayerPrefs.GetString(key);
            string d = Decryption(savedValue);
            Serialzer<T> serialzer = null;

            if (String.Equals(d, savedValue))
            {
                //decryption failed
                serialzer = JsonUtility.FromJson<Serialzer<T>>(d);
            }

            if (serialzer != null)
            {
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

            object retunValue = null;

            returnType = new ReturnType();

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

                string decryptedString = Decryption(json);

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
        public static void SetInt(string key, int value, bool useEncryption = false)
        {
            if (useEncryption)
            {
                Serialzer<int> serialzer = new Serialzer<int>();
                serialzer.type = PlayerPrefsType.Int;
                serialzer.value = value;
                serialzer.isEncrypted = useEncryption;
                string jsonString = JsonUtility.ToJson(serialzer);

                if(TryEncryption(jsonString,out string output))
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
        }
        internal static int StringToInt(string s)
        {
            return int.Parse(s);
        }
        public static void SetFloat(string key, float value, bool useEncryption = false)
        {
            if (useEncryption)
            {
                Serialzer<float> serialzer = new Serialzer<float>();
                serialzer.type = PlayerPrefsType.Float;
                serialzer.isEncrypted = useEncryption;

                serialzer.value = value;
                string jsonString = JsonUtility.ToJson(serialzer);

                if (TryEncryption(jsonString, out string output))
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
        }
        public static void SetFloat(string key, string value, bool useEncryption = false)
        {
            var floatValue = float.Parse(value);
            if (useEncryption)
            {
                Serialzer<float> serialzer = new Serialzer<float>();
                serialzer.type = PlayerPrefsType.Float;
                serialzer.value = floatValue;
                serialzer.isEncrypted = useEncryption;

                string jsonString = JsonUtility.ToJson(serialzer);

                if (TryEncryption(jsonString, out string output))
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
        internal static float StringToFloat(string s)
        {
            return float.Parse(s);
        }
        public static void SetString(string key, string value, bool useEncryption = false)
        {
            if (useEncryption)
            {
                Serialzer<string> serialzer = new Serialzer<string>();
                serialzer.type = PlayerPrefsType.String;
                serialzer.value = value;
                serialzer.isEncrypted = useEncryption;

                string jsonString = JsonUtility.ToJson(serialzer);

                if (TryEncryption(jsonString, out string output))
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
        }
        public static void SetVector3(string key, Vector3 _value, bool useEncryption = false)
        {
            Serialzer<Vector3> serialzer = new Serialzer<Vector3>();
            serialzer.type = PlayerPrefsType.Vector3;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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
        }
        internal static Vector3 StringToVector3(string s)
        {
            Vector3 outVector3 = Vector3.zero;

            if (s.Contains("{"))
            {
                outVector3 = JsonUtility.FromJson<Vector3>(s);
            }
            else
            {
                s = s.Replace("(", "");
                s = s.Replace(")", "");

                var splitString = s.Split(","[0]);

                // Build new Vector3 from array elements

                outVector3.x = float.Parse(splitString[0]);
                outVector3.y = float.Parse(splitString[1]);
                outVector3.z = float.Parse(splitString[2]);
            }

            return outVector3;
        }

        public static void SetVector3Int(string key, Vector3Int _value, bool useEncryption = false)
        {
            Serialzer<Vector3Int> serialzer = new Serialzer<Vector3Int>();
            serialzer.type = PlayerPrefsType.Vector3Int;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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
        }
        internal static Vector3Int StringToVector3Int(string s)
        {
            Vector3 outVector3 = Vector3.zero;

            if (s.Contains("{"))
            {
                outVector3 = JsonUtility.FromJson<Vector3>(s);
            }
            else
            {
                //Debug.Log(s);

                s = s.Replace("(", "");
                s = s.Replace(")", "");

                var splitString = s.Split(","[0]);

                // Build new Vector3 from array elements

                outVector3.x = float.Parse(splitString[0]);
                outVector3.y = float.Parse(splitString[1]);
                outVector3.z = float.Parse(splitString[2]);
            }

            return new Vector3Int((int)outVector3.x, (int)outVector3.y, (int)outVector3.z);
        }
        public static void SetByte(string key, byte _value, bool useEncryption = false)
        {
            Serialzer<byte> serialzer = new Serialzer<byte>();
            serialzer.type = PlayerPrefsType.Byte;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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
        }
        internal static byte StringToByte(string s)
        {
            return Byte.Parse(s);
        }
        public static void SetDoube(string key, double _value, bool useEncryption = false)
        {
            Serialzer<double> serialzer = new Serialzer<double>();
            serialzer.type = PlayerPrefsType.Double;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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
        }
        internal static double StringToDouble(string s)
        {
            return double.Parse(s);
        }
        public static void SetBool(string key, bool _value, bool useEncryption = false)
        {
            Serialzer<bool> serialzer = new Serialzer<bool>();
            serialzer.type = PlayerPrefsType.Bool;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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
        }
        internal static bool StringToBool(string s)
        {
            bool outBool = false;

            if (s == "True")
            {
                outBool = true;
            }
            return outBool;
        }
        public static void SetVector2(string key, Vector2 _value, bool useEncryption = false)
        {
            Serialzer<Vector2> serialzer = new Serialzer<Vector2>();
            serialzer.type = PlayerPrefsType.Vector2;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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

        }
        internal static Vector2 StringToVector2(string s)
        {
            Vector2 outVector3 = Vector2.zero;

            if (s.Contains("{"))
            {
                outVector3 = JsonUtility.FromJson<Vector2>(s);
            }
            else
            {
                //Debug.Log(s);

                s = s.Replace("(", "");
                s = s.Replace(")", "");

                var splitString = s.Split(","[0]);

                // Build new Vector3 from array elements

                outVector3.x = float.Parse(splitString[0]);
                outVector3.y = float.Parse(splitString[1]);
            }

            return outVector3;
        }

        public static void SetVector2Int(string key, Vector2Int _value, bool useEncryption = false)
        {
            Serialzer<Vector2Int> serialzer = new Serialzer<Vector2Int>();
            serialzer.type = PlayerPrefsType.Vector2Int;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;


            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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
        }
        internal static Vector2Int StringToVector2Int(string s)
        {
            //  Debug.Log("Vector2Int : " + s);

            Vector2 outVector3 = Vector2.zero;

            if (s.Contains("{"))
            {
                outVector3 = JsonUtility.FromJson<Vector2>(s);
            }
            else
            {
                s = s.Replace("(", "");
                s = s.Replace(")", "");

                var splitString = s.Split(","[0]);

                // Build new Vector3 from array elements

                outVector3.x = int.Parse(splitString[0]);
                outVector3.y = int.Parse(splitString[1]);
            }

            return new Vector2Int((int)outVector3.x, (int)outVector3.y);
        }

        public static void SetVector4(string key, Vector4 _value, bool useEncryption = false)
        {
            Serialzer<Vector4> serialzer = new Serialzer<Vector4>();
            serialzer.type = PlayerPrefsType.Vector4;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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
        }
        internal static Vector4 StringToVector4(string s)
        {
            Vector4 outVector3 = Vector4.zero;

            if (s.Contains("{"))
            {
                outVector3 = JsonUtility.FromJson<Vector4>(s);
            }
            else
            {
                s = s.Replace("(", "");
                s = s.Replace(")", "");

                var splitString = s.Split(","[0]);
                outVector3.x = float.Parse(splitString[0]);
                outVector3.y = float.Parse(splitString[1]);
                outVector3.z = float.Parse(splitString[2]);
                outVector3.w = float.Parse(splitString[3]);
            }

            return outVector3;
        }
        public static void SetColor(string key, Color _value, bool hdr, bool useEncryption = false)
        {
            Serialzer<Color> serialzer = new Serialzer<Color>();
            serialzer.isEncrypted = useEncryption;

            if (hdr)
            {
                serialzer.type = PlayerPrefsType.HDRColor;
            }
            else
            {
                serialzer.type = PlayerPrefsType.Color;
            }


            serialzer.value = _value;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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
        }
        internal static Color StringToColor(string s)
        {
            Color outColor = Color.white;

            if (ColorUtility.TryParseHtmlString("#" + s, out Color _Color))
            {
                outColor = _Color;
            }
            else if (s.Contains("{"))
            {
                s = s.Replace("{", "");
                s = s.Replace("}", "");
                s = s.Replace('"', ' ');
                s = s.Replace("r", "");
                s = s.Replace("g", "");
                s = s.Replace("b", "");
                s = s.Replace("a", "");
                s = s.Replace(":", "");
                s = s.Replace(" ", "");

                var splitString = s.Split(","[0]);


                //    // Build new Vector3 from array elements

                outColor.r = float.Parse(splitString[0]);
                outColor.g = float.Parse(splitString[1]);
                outColor.b = float.Parse(splitString[2]);
                outColor.a = float.Parse(splitString[3]);

            }
            else
            {
                s = s.Replace("RGBA", "");
                s = s.Replace("#", "");
                s = s.Replace("(", "");
                s = s.Replace(")", "");

                var splitString = s.Split(","[0]);


                //    // Build new Vector3 from array elements

                outColor.r = float.Parse(splitString[0]);
                outColor.g = float.Parse(splitString[1]);
                outColor.b = float.Parse(splitString[2]);
                outColor.a = float.Parse(splitString[3]);
            }

            return outColor;
        }

        public static void SetDateTime(string key, DateTime _value, bool useEncryption = false)
        {
            Serialzer<DateTime> serialzer = new Serialzer<DateTime>();

            serialzer.type = PlayerPrefsType.DateTime;

            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;
            string jsonString = JsonConvert.SerializeObject(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
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
        }
        internal static DateTime? StringToDateTime(string s)
        {
            if (DateTime.TryParse(s, out DateTime d))
            {
                return d;
            }
            else
            {
                return null;
            }
        }
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

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

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
        private static bool TryParseJson<T>(this string @this, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };

            result = JsonConvert.DeserializeObject<T>(@this, settings);
            return success;
        }
        internal static bool TryEncryption(string inputData,out string _result)
        {
            Init();

            if (EncryptionSettings == null)
            {
                _result = inputData;
                return false;
            }

            AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
            AEScryptoProvider.BlockSize = 128;
            AEScryptoProvider.KeySize = 256;
            AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(EncryptionSettings.GetKey());
            AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(EncryptionSettings.Getiv());
            AEScryptoProvider.Mode = CipherMode.CBC;
            AEScryptoProvider.Padding = PaddingMode.PKCS7;

            byte[] txtByteData = ASCIIEncoding.ASCII.GetBytes(inputData);
            ICryptoTransform trnsfrm = AEScryptoProvider.CreateEncryptor(AEScryptoProvider.Key, AEScryptoProvider.IV);

            byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
            _result = Convert.ToBase64String(result);
            return true;
        }

        internal static string Decryption(string inputData)
        {
            Init();
            string returnstring = inputData;
            if (EncryptionSettings == null)
            {
                return returnstring;
            }
            try
            {
                AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
                AEScryptoProvider.BlockSize = 128;
                AEScryptoProvider.KeySize = 256;
                AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(EncryptionSettings.GetKey());
                AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(EncryptionSettings.Getiv());
                AEScryptoProvider.Mode = CipherMode.CBC;
                AEScryptoProvider.Padding = PaddingMode.PKCS7;

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
                string ex = e.ToString();
            }

            return returnstring;
        }

        internal static void SetAPPsCSDK(string APPsCSDK)
        {
            SetString(AdvancedPlayerPrefsGlobalVariables.CryptoSaveKey, APPsCSDK);
        }
        internal static bool HasAPPsCSDK()
        {
            return HasKey(AdvancedPlayerPrefsGlobalVariables.CryptoSaveKey);
        }
        internal static string GetAPPsCSDK()
        {
            return GetString(AdvancedPlayerPrefsGlobalVariables.CryptoSaveKey, string.Empty);
        }
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
}