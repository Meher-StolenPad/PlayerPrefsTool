using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text.RegularExpressions;
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
        Long,

        Vector2,
        Vector2Int,
        Vector3,
        Vector3Int,
        Vector4,

        Color,
        HDRColor,

        DateTime,

        ArrayInt,
        ArrayFloat,
        ArrayString,
        ArrayDouble,
        ArrayLong,
        ArrayBool,
        ArrayByte,
        ArrayVector3,
        ArrayVector3Int,
        ArrayVector2,
        ArrayVector2Int,
        ArrayVector4
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
        private static string numberPattern = " ({0})";
        private static bool showEncryptionWarning;
        private static bool showDecryptionWarning;
        #endregion
        //DavanciDebug.Warning(AdvancedPlayerPrefsGlobalVariables.NoEncryptionSettingsWarning);
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
        public static int GetInt(string key, int defaultValue = int.MinValue)
        {
            int returnInt = PlayerPrefs.GetInt(key, defaultValue);

            if (returnInt == defaultValue)
            {
                returnInt = GetCosutomTypeValue<int>(key, defaultValue);
            }

            return returnInt;
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
            return Decryption(PlayerPrefs.GetString(key, defaultValue));
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
        public static bool GetBool(string key, bool defaultValue = false)
        {
            return GetCosutomTypeValue<bool>(key, defaultValue);
        }
        public static byte GetByte(string key, byte defaultValue = byte.MinValue)
        {
            return GetCosutomTypeValue<byte>(key, defaultValue);
        }
        public static double GetDouble(string key, double defaultValue = double.MinValue)
        {
            return GetCosutomTypeValue<double>(key, defaultValue);
        }
        public static long Getlong(string key, long defaultValue = long.MinValue)
        {
            return GetCosutomTypeValue<long>(key, defaultValue);
        }
        public static Vector2Int GetVector2Int(string key, Vector2Int defaultValue)
        {
            return GetCosutomTypeValue<Vector2Int>(key, defaultValue);
        }
        public static Vector3Int GetVector3Int(string key, Vector3Int defaultValue)
        {
            return GetCosutomTypeValue<Vector3Int>(key, defaultValue);
        }
        public static int[] GetArray(string key, int[] defaultValue = null)
        {
            var effectiveEnd = defaultValue ?? new int[0];
            return GetCosutomTypeValue<int[]>(key, effectiveEnd);
        }
        public static List<int> GetList(string key, List<int> defaultValue)
        {
            var t = GetCosutomTypeValue<int[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static float[] GetArray(string key, float[] defaultValue)
        {
            return GetCosutomTypeValue<float[]>(key, defaultValue);
        }
        public static List<float> GetList(string key, List<float> defaultValue)
        {
            var t = GetCosutomTypeValue<float[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static string[] GetArray(string key, string[] defaultValue)
        {
            return GetCosutomTypeValue<string[]>(key, defaultValue);
        }
        public static List<string> GetList(string key, List<string> defaultValue)
        {
            var t = GetCosutomTypeValue<string[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static bool[] GetArray(string key, bool[] defaultValue)
        {
            return GetCosutomTypeValue<bool[]>(key, defaultValue);
        }
        public static List<bool> GetList(string key, List<bool> defaultValue)
        {
            var t = GetCosutomTypeValue<bool[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static double[] GetArray(string key, double[] defaultValue)
        {
            return GetCosutomTypeValue<double[]>(key, defaultValue);
        }
        public static List<double> GetList(string key, List<double> defaultValue)
        {
            var t = GetCosutomTypeValue<double[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static long[] GetArray(string key, long[] defaultValue)
        {
            return GetCosutomTypeValue<long[]>(key, defaultValue);
        }
        public static List<long> GetList(string key, List<long> defaultValue)
        {
            var t = GetCosutomTypeValue<long[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static byte[] GetArray(string key, byte[] defaultValue)
        {
            return GetCosutomTypeValue<byte[]>(key, defaultValue);
        }
        public static List<byte> GetList(string key, List<byte> defaultValue)
        {
            var t = GetCosutomTypeValue<byte[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static Vector3[] GetArray(string key, Vector3[] defaultValue)
        {
            return GetCosutomTypeValue<Vector3[]>(key, defaultValue);
        }
        public static List<Vector3> GetList(string key, List<Vector3> defaultValue)
        {
            var t = GetCosutomTypeValue<Vector3[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static Vector3Int[] GetArray(string key, Vector3Int[] defaultValue)
        {
            return GetCosutomTypeValue<Vector3Int[]>(key, defaultValue);
        }
        public static List<Vector3Int> GetList(string key, List<Vector3Int> defaultValue)
        {
            var t = GetCosutomTypeValue<Vector3Int[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static Vector2[] GetArray(string key, Vector2[] defaultValue)
        {
            return GetCosutomTypeValue<Vector2[]>(key, defaultValue);
        }
        public static List<Vector2> GetList(string key, List<Vector2> defaultValue)
        {
            var t = GetCosutomTypeValue<Vector2[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static Vector2Int[] GetArray(string key, Vector2Int[] defaultValue)
        {
            return GetCosutomTypeValue<Vector2Int[]>(key, defaultValue);
        }
        public static List<Vector2Int> GetList(string key, List<Vector2Int> defaultValue)
        {
            var t = GetCosutomTypeValue<Vector2Int[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        public static Vector4[] GetArray(string key, Vector4[] defaultValue)
        {
            return GetCosutomTypeValue<Vector4[]>(key, defaultValue);
        }
        public static List<Vector4> GetList(string key, List<Vector4> defaultValue)
        {
            var t = GetCosutomTypeValue<Vector4[]>(key, defaultValue.ToArray());
            return t.ToList();
        }
        #endregion

        #region Set Variable Region
        public static void SetInt(string key, int value, bool useEncryption = false)
        {
            if (useEncryption)
            {
                Serialzer<int> serialzer = new Serialzer<int>();
                serialzer.type = PlayerPrefsType.Int;
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
                    PlayerPrefs.SetInt(key, value);
                }
            }
            else
            {
                PlayerPrefs.SetInt(key, value);
            }
            DavanciDebug.Log("Set Int : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set Float : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set String : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set Vector 3 : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set Vector 3 Int: " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set Byte : " + key + ", Use Encryption : " + useEncryption, Color.cyan);

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
            DavanciDebug.Log("Set Double : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetLong(string key, long _value, bool useEncryption = false)
        {
            Serialzer<long> serialzer = new Serialzer<long>();
            serialzer.type = PlayerPrefsType.Long;
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
            DavanciDebug.Log("Set Long : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set Bool : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set Vector 2 : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set Vector 2 Int: " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set Vector 4: " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set Color: " + key + ", Use Encryption : " + useEncryption, Color.cyan);
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
            DavanciDebug.Log("Set DateTime: " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, int[] _value, bool useEncryption = false)
        {
            Serialzer<int[]> serialzer = new Serialzer<int[]>();
            serialzer.type = PlayerPrefsType.ArrayInt;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;
            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set int Array: " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<int> _value, bool useEncryption = false)
        {
            Serialzer<int[]> serialzer = new Serialzer<int[]>();
            serialzer.type = PlayerPrefsType.ArrayInt;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set int List: " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, float[] _value, bool useEncryption = false)
        {
            Serialzer<float[]> serialzer = new Serialzer<float[]>();
            serialzer.type = PlayerPrefsType.ArrayFloat;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);

            DavanciDebug.Log("Set float Array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<float> _value, bool useEncryption = false)
        {
            Serialzer<float[]> serialzer = new Serialzer<float[]>();
            serialzer.type = PlayerPrefsType.ArrayFloat;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set float list : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, string[] _value, bool useEncryption = false)
        {
            Serialzer<string[]> serialzer = new Serialzer<string[]>();
            serialzer.type = PlayerPrefsType.ArrayString;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set string Array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<string> _value, bool useEncryption = false)
        {
            Serialzer<string[]> serialzer = new Serialzer<string[]>();
            serialzer.type = PlayerPrefsType.ArrayString;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set string List : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, bool[] _value, bool useEncryption = false)
        {
            Serialzer<bool[]> serialzer = new Serialzer<bool[]>();
            serialzer.type = PlayerPrefsType.ArrayBool;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    Debug.Log("Array : " + output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Bool array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<bool> _value, bool useEncryption = false)
        {
            Serialzer<bool[]> serialzer = new Serialzer<bool[]>();
            serialzer.type = PlayerPrefsType.ArrayBool;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Bool List : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, byte[] _value, bool useEncryption = false)
        {
            Serialzer<byte[]> serialzer = new Serialzer<byte[]>();
            serialzer.type = PlayerPrefsType.ArrayByte;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Byte Array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<byte> _value, bool useEncryption = false)
        {
            Serialzer<byte[]> serialzer = new Serialzer<byte[]>();
            serialzer.type = PlayerPrefsType.ArrayByte;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Byte List : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, double[] _value, bool useEncryption = false)
        {
            Serialzer<double[]> serialzer = new Serialzer<double[]>();
            serialzer.type = PlayerPrefsType.ArrayDouble;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Double Array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<double> _value, bool useEncryption = false)
        {
            Serialzer<double[]> serialzer = new Serialzer<double[]>();
            serialzer.type = PlayerPrefsType.ArrayDouble;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Double List : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, long[] _value, bool useEncryption = false)
        {
            Serialzer<long[]> serialzer = new Serialzer<long[]>();
            serialzer.type = PlayerPrefsType.ArrayLong;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Long Array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<long> _value, bool useEncryption = false)
        {
            Serialzer<long[]> serialzer = new Serialzer<long[]>();
            serialzer.type = PlayerPrefsType.ArrayLong;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Long List : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, Vector3[] _value, bool useEncryption = false)
        {
            Serialzer<Vector3[]> serialzer = new Serialzer<Vector3[]>();
            serialzer.type = PlayerPrefsType.ArrayVector3;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector3 Array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<Vector3> _value, bool useEncryption = false)
        {
            Serialzer<Vector3[]> serialzer = new Serialzer<Vector3[]>();
            serialzer.type = PlayerPrefsType.ArrayVector3;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector3 List : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, Vector3Int[] _value, bool useEncryption = false)
        {
            Serialzer<Vector3Int[]> serialzer = new Serialzer<Vector3Int[]>();
            serialzer.type = PlayerPrefsType.ArrayVector3Int;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector3 Int Array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<Vector3Int> _value, bool useEncryption = false)
        {
            Serialzer<Vector3Int[]> serialzer = new Serialzer<Vector3Int[]>();
            serialzer.type = PlayerPrefsType.ArrayVector3Int;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector3 Int List : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, Vector2[] _value, bool useEncryption = false)
        {
            Serialzer<Vector2[]> serialzer = new Serialzer<Vector2[]>();
            serialzer.type = PlayerPrefsType.ArrayVector2;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector2 Array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<Vector2> _value, bool useEncryption = false)
        {
            Serialzer<Vector2[]> serialzer = new Serialzer<Vector2[]>();
            serialzer.type = PlayerPrefsType.ArrayVector2;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector2 List : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, Vector2Int[] _value, bool useEncryption = false)
        {
            Serialzer<Vector2Int[]> serialzer = new Serialzer<Vector2Int[]>();
            serialzer.type = PlayerPrefsType.ArrayVector2Int;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector2 Int Array : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<Vector2Int> _value, bool useEncryption = false)
        {
            Serialzer<Vector2Int[]> serialzer = new Serialzer<Vector2Int[]>();
            serialzer.type = PlayerPrefsType.ArrayVector2Int;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector2 Int List : " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetArray(string key, Vector4[] _value, bool useEncryption = false)
        {
            Serialzer<Vector4[]> serialzer = new Serialzer<Vector4[]>();
            serialzer.type = PlayerPrefsType.ArrayVector4;
            serialzer.value = _value;
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);
            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector4 Array: " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        public static void SetList(string key, List<Vector4> _value, bool useEncryption = false)
        {
            Serialzer<Vector4[]> serialzer = new Serialzer<Vector4[]>();
            serialzer.type = PlayerPrefsType.ArrayVector4;
            serialzer.value = _value.ToArray();
            serialzer.isEncrypted = useEncryption;

            string jsonString = JsonUtility.ToJson(serialzer);

            if (useEncryption)
            {
                if (TryEncryption(jsonString, out string output))
                {
                    serialzer.isEncrypted = false;
                    PlayerPrefs.SetString(key, output);
                    return;
                }
            }
            PlayerPrefs.SetString(key, jsonString);
            DavanciDebug.Log("Set Vector4 List: " + key + ", Use Encryption : " + useEncryption, Color.cyan);
        }
        #endregion

        #region Json Data Region / Internal data 
        private static T GetCosutomTypeValue<T>(string key, T defaultValue = default)
        {
            object returnvalue = default;

            string savedValue = PlayerPrefs.GetString(key);

            string d = Decryption(savedValue);

            Serialzer<T> serialzer = null;

            if (d.TryParseJson(out Serialzer<T> t))
            {
                serialzer = t;
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
        internal static byte StringToByte(string s)
        {
            return Byte.Parse(s);
        }
        internal static double StringToDouble(string s)
        {
            return double.Parse(s);
        }
        internal static long StringToLong(string s)
        {
            return long.Parse(s);
        }
        internal static bool StringToBool(string s)
        {
            bool outBool = bool.Parse(s);
            return outBool;
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
        internal static int[] StringToArrayInt(string s)
        {
            int[] outVector3 = null;

            s = s.Replace("[", "");
            s = s.Replace("]", "");

            var splitString = s.Split(","[0]);
            List<int> returnlist = new List<int>();
            foreach (var item in splitString)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int t = int.Parse(item);
                    returnlist.Add(t);
                }

            }
            outVector3 = returnlist.ToArray();
            return outVector3;
        }
        internal static long[] StringToArrayLong(string s)
        {
            long[] outVector3 = null;

            s = s.Replace("[", "");
            s = s.Replace("]", "");

            var splitString = s.Split(","[0]);
            List<long> returnlist = new List<long>();
            foreach (var item in splitString)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    long t = long.Parse(item);
                    returnlist.Add(t);
                }

            }
            outVector3 = returnlist.ToArray();
            return outVector3;
        }
        internal static float[] StringToArrayFloat(string s)
        {
            float[] outVector3 = null;

            s = s.Replace("[", "");
            s = s.Replace("]", "");

            var splitString = s.Split(","[0]);
            List<float> returnlist = new List<float>();
            foreach (var item in splitString)
            {
                float t = float.Parse(item);
                returnlist.Add(t);
            }
            outVector3 = returnlist.ToArray();
            return outVector3;
        }
        internal static string[] StringToArrayString(string s)
        {
            string[] outVector3 = new string[0];
            //[
            //  "test",
            //  "Test2"
            //]
            s = s.Replace("[", "");
            s = s.Replace("]", "");

            var regex = new Regex("\".*?\"");
            var matches = regex.Matches(s);

            List<string> returnlist = new List<string>();
            foreach (var item in matches)
            {
                string t = item.ToString();
                t = t.Remove(0, 1);
                t = t.Remove(t.Length - 1, 1);
                returnlist.Add(t);
            }
            outVector3 = returnlist.ToArray();
            return outVector3;
        }
        internal static bool[] StringToArrayBool(string s)
        {
            bool[] outVector3 = null;

            s = s.Replace("[", "");
            s = s.Replace("]", "");

            var splitString = s.Split(","[0]);
            List<bool> returnlist = new List<bool>();
            foreach (var item in splitString)
            {
                bool t = bool.Parse(item);
                returnlist.Add(t);
            }
            outVector3 = returnlist.ToArray();
            return outVector3;
        }
        internal static byte[] StringToArrayByte(string s)
        {
            byte[] outVector3 = null;

            s = s.Replace("[", "");
            s = s.Replace("]", "");

            var splitString = s.Split(","[0]);
            List<byte> returnlist = new List<byte>();
            foreach (var item in splitString)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    byte t = byte.Parse(item);
                    returnlist.Add(t);
                }

            }
            outVector3 = returnlist.ToArray();
            return outVector3;
        }
        internal static double[] StringToArrayDouble(string s)
        {
            double[] outVector3 = null;

            s = s.Replace("[", "");
            s = s.Replace("]", "");

            var splitString = s.Split(","[0]);
            List<double> returnlist = new List<double>();
            foreach (var item in splitString)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    double t = double.Parse(item);
                    returnlist.Add(t);

                }
            }
            outVector3 = returnlist.ToArray();
            return outVector3;
        }
        internal static Vector3[] StringToArrayVector3(string s)
        {
            Vector3[] outVector3 = null;

            var regex = new Regex("(?<={)[^}]*(?=})");
            var matches = regex.Matches(s);
            List<Vector3> vectors = new List<Vector3>();

            foreach (var item in matches)
            {
                string t = "{" + item.ToString() + "}";
                t = t.Replace('\n', ' ');
                t = t.Replace(" ", "");
                Vector3 tt = JsonUtility.FromJson<Vector3>(t);
                vectors.Add(tt);
            }
            outVector3 = vectors.ToArray();

            return outVector3;
        }
        internal static Vector3Int[] StringToArrayVector3Int(string s)
        {
            Vector3Int[] outVector3 = null;

            var regex = new Regex("(?<={)[^}]*(?=})");
            var matches = regex.Matches(s);
            List<Vector3Int> vectors = new List<Vector3Int>();

            foreach (var item in matches)
            {
                string t = "{" + item.ToString() + "}";
                t = t.Replace('\n', ' ');
                t = t.Replace(" ", "");
                Vector3Int tt = Vector3Int.FloorToInt(JsonUtility.FromJson<Vector3>(t));
                vectors.Add(tt);
            }
            outVector3 = vectors.ToArray();

            return outVector3;
        }
        internal static Vector2[] StringToArrayVector2(string s)
        {
            Vector2[] outVector3 = null;

            var regex = new Regex("(?<={)[^}]*(?=})");
            var matches = regex.Matches(s);
            List<Vector2> vectors = new List<Vector2>();

            foreach (var item in matches)
            {
                string t = "{" + item.ToString() + "}";
                t = t.Replace('\n', ' ');
                t = t.Replace(" ", "");
                Vector2 tt = JsonUtility.FromJson<Vector2>(t);
                vectors.Add(tt);
            }
            outVector3 = vectors.ToArray();

            return outVector3;
        }
        internal static Vector2Int[] StringToArrayVector2Int(string s)
        {
            Vector2Int[] outVector3 = null;

            var regex = new Regex("(?<={)[^}]*(?=})");
            var matches = regex.Matches(s);
            List<Vector2Int> vectors = new List<Vector2Int>();

            foreach (var item in matches)
            {
                string t = "{" + item.ToString() + "}";
                t = t.Replace('\n', ' ');
                t = t.Replace(" ", "");
                Vector2Int tt = Vector2Int.FloorToInt(JsonUtility.FromJson<Vector2>(t));
                vectors.Add(tt);
            }
            outVector3 = vectors.ToArray();

            return outVector3;
        }
        internal static Vector4[] StringToArrayVector4(string s)
        {
            Vector4[] outVector3 = null;
            //
            var regex = new Regex("(?<={)[^}]*(?=})");

            var matches = regex.Matches(s);

            List<Vector4> vectors = new List<Vector4>();
            foreach (var item in matches)
            {
                string t = "{" + item.ToString() + "}";
                t = t.Replace('\n', ' ');
                t = t.Replace(" ", "");
                Vector4 tt = JsonUtility.FromJson<Vector4>(t);
                vectors.Add(tt);
            }
            outVector3 = vectors.ToArray();
            return outVector3;
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
        internal static bool TryEncryption(string inputData, out string _result)
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

            AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
            AEScryptoProvider.BlockSize = 128;
            AEScryptoProvider.KeySize = 256;
            AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(AdvancedPlayerPrefsSettings.Instance.GetKey());
            AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(AdvancedPlayerPrefsSettings.Instance.Getiv());
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
                AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
                AEScryptoProvider.BlockSize = 128;
                AEScryptoProvider.KeySize = 256;
                AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(AdvancedPlayerPrefsSettings.Instance.GetKey());
                AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(AdvancedPlayerPrefsSettings.Instance.Getiv());
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
            SetString(AdvancedPlayerPrefsGlobalVariables.APPsCSDK, APPsCSDK);
        }
        internal static bool HasAPPsCSDK()
        {
            return HasKey(AdvancedPlayerPrefsGlobalVariables.APPsCSDK);
        }
        internal static string GetAPPsCSDK()
        {
            return GetString(AdvancedPlayerPrefsGlobalVariables.APPsCSDK, string.Empty);
        }
        #endregion
    }
}