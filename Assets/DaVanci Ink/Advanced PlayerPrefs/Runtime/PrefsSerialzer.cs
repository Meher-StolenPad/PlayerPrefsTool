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
namespace DaVanciInk.AdvancedPlayerPrefs
{
    public enum PlayerPrefsType
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
    public class ReturnType
    {
        public PlayerPrefsType PlayerPrefsType = PlayerPrefsType.String;
        public bool IsEncrypted;
    }
    public static class PrefsSerialzer
    {
        private static string numberPattern = " ({0})";
        private static EncryptionSettings EncryptionSettings;

        private static void GetKeys()
        {
            if (EncryptionSettings == null)
            {
                EncryptionSettings = Resources.Load<EncryptionSettings>("AdvancedPlayerPrefs/EncryptionSettings");

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
        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        public static float GetFloat(string key, float defaultValue = 0.0f)
        {
            float returnFloat = PlayerPrefs.GetFloat(key, defaultValue);

            if(returnFloat == defaultValue)
            {
                returnFloat= GetCosutomTypeValue<float>(key, defaultValue);
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
            string d=  Decryption(PlayerPrefs.GetString(key));
            Serialzer<T> serialzer = JsonUtility.FromJson<Serialzer<T>>(d);
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
        public static string ReturnObjectValue(Serialzer<object> data)
        {
            return data.value.ToString();
        }
        public static object TryGetCostumeType(string key, out ReturnType returnType, string defaultValue = "")
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
        public static void SetInt(string key, int value, bool useEncryption=false)
        {
            Debug.Log("SetInt : " + value);
            if (useEncryption)
            {
                Serialzer<int> serialzer = new Serialzer<int>();
                serialzer.type = PlayerPrefsType.Int;
                serialzer.value = value;
                string jsonString = JsonUtility.ToJson(serialzer);

                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetInt(key, value);
            }
        }
        public static int StringToInt(string s)
        {
            return int.Parse(s);
        }
        public static void SetFloat(string key, float value, bool useEncryption = false)
        {
            if (useEncryption)
            {
                Serialzer<float> serialzer = new Serialzer<float>();
                serialzer.type = PlayerPrefsType.Float;
                serialzer.value = value;
                string jsonString = JsonUtility.ToJson(serialzer);

                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetFloat(key, value);
            }
        }
        public static void SetFloat(string key, string value, bool useEncryption=false)
        {
            var floatValue = float.Parse(value);

            Debug.Log("SetFloat : " + value);
            if (useEncryption)
            {
                Serialzer<float> serialzer = new Serialzer<float>();
                serialzer.type = PlayerPrefsType.Float;
                serialzer.value = floatValue;
                string jsonString = JsonUtility.ToJson(serialzer);

                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetFloat(key, floatValue);
            }
        }
        public static float StringToFloat(string s)
        {
            return float.Parse(s);    
        }
        public static void SetString(string key, string value, bool useEncryption=false)
        {
            Debug.Log("SetString : " + value);
            if (useEncryption)
            {
                Serialzer<string> serialzer = new Serialzer<string>();
                serialzer.type = PlayerPrefsType.String;
                serialzer.value = value;
                string jsonString = JsonUtility.ToJson(serialzer);

                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
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

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
        }
        public static Vector3 StringToVector3(string s)
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

            return outVector3;
        }

        public static void SetVector3Int(string key, Vector3Int _value, bool useEncryption = false)
        {
            Serialzer<Vector3Int> serialzer = new Serialzer<Vector3Int>();
            serialzer.type = PlayerPrefsType.Vector3Int;
            serialzer.value = _value;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
        }
        public static Vector3Int StringToVector3Int(string s)
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

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
        }
        public static byte StringToByte(string s)
        {
            return Byte.Parse(s);
        }
        public static void SetDoube(string key, double _value, bool useEncryption = false)
        {
            Serialzer<double> serialzer = new Serialzer<double>();
            serialzer.type = PlayerPrefsType.Double;
            serialzer.value = _value;

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
        }
        public static double StringToDouble(string s)
        {
            return double.Parse(s);
        }
        public static void SetBool(string key, bool _value, bool useEncryption = false)
        {
            Serialzer<bool> serialzer = new Serialzer<bool>();
            serialzer.type = PlayerPrefsType.Bool;
            serialzer.value = _value;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
        }
        public static bool StringToBool(string s)
        {
            bool outBool = false;

            if (s == "True")
            {
                outBool = true;
            }
            return outBool;
        }
        public static void SetVector2(string key, Vector2 _value, bool useEncryption=false)
        {
            Serialzer<Vector2> serialzer = new Serialzer<Vector2>();
            serialzer.type = PlayerPrefsType.Vector2;
            serialzer.value = _value;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }

        }
        public static Vector2 StringToVector2(string s)
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


            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
        }
        public static Vector2Int StringToVector2Int(string s)
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

        public static void SetVector4(string key, Vector4 _value,bool useEncryption = false)
        {
            Serialzer<Vector4> serialzer = new Serialzer<Vector4>();
            serialzer.type = PlayerPrefsType.Vector4;
            serialzer.value = _value;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
        }
        public static Vector4 StringToVector4(string s)
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
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
        }
        public static Color StringToColor(string s)
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
            //JsonConvert.SerializeObject(DateTime.Now)
            string jsonString = JsonConvert.SerializeObject(serialzer);
            if (useEncryption)
            {
                string encryptedString = Encryption(jsonString);
                PlayerPrefs.SetString(key, encryptedString);
            }
            else
            {
                PlayerPrefs.SetString(key, jsonString);
            }
        }
        public static DateTime? StringToDateTime(string s)
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
        public static void CopyToClipboard(this string s)
        {
            TextEditor te = new TextEditor();
            te.text = s;
            te.SelectAll();
            te.Copy();
        }
        public static string NextAvailableFilename(string path)
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
        private static string GetNextFilename(string pattern)
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
        public static bool ValidateJSON(this string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
        }
        public static bool TryParseJson<T>(this string @this, out T result)
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
        public static bool IsValidJson(this string src)
        {
            try
            {
                var asToken = JToken.Parse(src);
                JArray array = JArray.Parse(src);
                foreach (JObject content in array.Children<JObject>())
                {
                    foreach (JProperty prop in content.Properties())
                    {
                        Debug.Log(prop.Name);
                    }
                }
                return asToken.Type == JTokenType.Object || asToken.Type == JTokenType.Array;
            }
            catch (Exception)  // Typically a JsonReaderException exception if you want to specify.
            {
                return false;
            }
        }
        public static string Encryption(string inputData)
        {
            GetKeys();

            AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
            AEScryptoProvider.BlockSize = 128;
            AEScryptoProvider.KeySize = 256;
            AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(EncryptionSettings.Key);
            AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(EncryptionSettings.Lv);
            AEScryptoProvider.Mode = CipherMode.CBC;
            AEScryptoProvider.Padding = PaddingMode.PKCS7;

            byte[] txtByteData = ASCIIEncoding.ASCII.GetBytes(inputData);
            ICryptoTransform trnsfrm = AEScryptoProvider.CreateEncryptor(AEScryptoProvider.Key, AEScryptoProvider.IV);

            byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
            return Convert.ToBase64String(result);
        }

        public static string Decryption(string inputData)
        {
            string returnstring = inputData;
            GetKeys();

            try
            {
                AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
                AEScryptoProvider.BlockSize = 128;
                AEScryptoProvider.KeySize = 256;
                AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(EncryptionSettings.Key);
                AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(EncryptionSettings.Lv);
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
                }
            }
            catch (InvalidCastException e)
            {
            }

            return returnstring;
        }


    }
    [Serializable]
    public class Serialzer<T>
    {
        public bool isEncrypted;
        public PlayerPrefsType TypeBeforeEncryption;
        public PlayerPrefsType type;
        [SerializeField]
        public T value;
    }
    [Serializable]
    public class ExportSerialzer
    {
        public string key;
        public PlayerPrefsType type;
        public string value;
    }
    [Serializable]
    public class ExportSerialzerHolder
    {
        public List<ExportSerialzer> exportlist = new List<ExportSerialzer>();
    }
}